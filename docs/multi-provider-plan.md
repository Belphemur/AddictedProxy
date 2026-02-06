# Multi-Provider Architecture Plan: Adding SuperSubtitles

## Goal

Enable AddictedProxy to support multiple subtitle providers beyond Addic7ed. The first new provider is **SuperSubtitles** (based on the [SuperSubtitles](https://github.com/Belphemur/SuperSubtitles) project, which scrapes feliratok.eu). Shows from different providers should be **merged** when they represent the same media, so the rest of the app (search, browse, download) works seamlessly regardless of the subtitle source.

## SuperSubtitles Provider Overview

SuperSubtitles is a Go-based scraper for feliratok.eu (a Hungarian subtitle site). It will expose an API (REST or gRPC) with these capabilities:

| Capability | Description |
|---|---|
| **Show List** | Returns `Show` objects: `{ id, name, year, imageUrl }` |
| **Subtitles** | Returns `SubtitleCollection` per show with season/episode numbers, language, uploader, quality, download URL |
| **Third-Party IDs** | IMDB, TVDB, TVMaze, Trakt IDs for show matching |
| **Update Check** | Check for new content since a given content ID |

**Key differences from Addic7ed:**
- Returns structured JSON (not HTML scraping)
- No credential/authentication system needed
- Provides third-party IDs natively (IMDB, TVDB)
- Includes video quality metadata
- Supports season packs

## Architecture Changes

### Phase 1: Database Schema Changes (EF Core Migration)

#### 1.1 Extend `DataSource` Enum

```csharp
public enum DataSource
{
    Addic7ed,
    SuperSubtitles
}
```

#### 1.2 Add Provider-Specific External ID Table

Currently, `TvShow.ExternalId` stores a single provider-specific ID (Addic7ed's show ID). With multiple providers, a single show may have different external IDs per provider.

**New Entity: `ShowExternalId`**

```
┌──────────────────────────────────────────┐
│          ShowExternalId                   │
├──────────────────────────────────────────┤
│ Id (PK, long)                            │
│ TvShowId (FK → TvShow)                   │
│ Source (DataSource enum)                 │
│ ExternalId (string)                      │  ◄── Provider-specific ID
│ CreatedAt / UpdatedAt (BaseEntity)       │
├──────────────────────────────────────────┤
│ Unique: (TvShowId, Source)               │  ◄── One external ID per provider per show
│ Index: (Source, ExternalId) unique        │  ◄── Fast lookup by provider + ID
└──────────────────────────────────────────┘
```

This replaces the current `TvShow.ExternalId` field for multi-provider lookups. The original `TvShow.ExternalId` can be preserved temporarily for backwards compatibility and migrated via a `OneTimeMigrationRelease`.

#### 1.3 Episode External IDs

Similarly, `Episode.ExternalId` needs to support multiple providers:

**New Entity: `EpisodeExternalId`**

```
┌──────────────────────────────────────────┐
│        EpisodeExternalId                  │
├──────────────────────────────────────────┤
│ Id (PK, long)                            │
│ EpisodeId (FK → Episode)                │
│ Source (DataSource enum)                 │
│ ExternalId (string)                      │
│ CreatedAt / UpdatedAt (BaseEntity)       │
├──────────────────────────────────────────┤
│ Unique: (EpisodeId, Source)              │
│ Index: (Source, ExternalId) unique        │
└──────────────────────────────────────────┘
```

#### 1.4 One-Time Data Migration

Use the `OneTimeMigration` framework to migrate existing data:

```csharp
[MigrationDate(2026, 2, 6)]
public class MigrateExternalIdsToNewTableMigration : IMigration
{
    // 1. Copy TvShow.ExternalId → ShowExternalId (Source = Addic7ed)
    // 2. Copy Episode.ExternalId → EpisodeExternalId (Source = Addic7ed)
}
```

### Phase 2: Data Merging Strategy (Shows, Episodes, Subtitles)

The core design principle is: **one `TvShow` row per real-world show, one `Episode` row per real-world episode, many `Subtitle` rows per episode (from any provider)**. This means searching for a show and its episodes automatically returns subtitles from all providers with zero code changes in the search/browse/download path.

#### 2.1 Show Merging

Shows from different providers that represent the same media must be merged into a single `TvShow` row. The merge key is a **third-party ID** that both providers share.

**Why this works:** SuperSubtitles provides TvDB/IMDB IDs natively via its `ThirdPartyIds` struct. Addic7ed shows get their TvDB/TMDB IDs populated by the existing `MapShowTmdbJob` → `FetchMissingTvdbIdJob` pipeline. Once both sides have TvDB IDs, matching is straightforward.

**Merge Logic (executed by the new `RefreshSuperSubtitlesJob`, see [Phase 4](#phase-4-background-job-pipeline)):**

```
For each show from SuperSubtitles API:
    │
    ├─ Extract ThirdPartyIds (TvDB ID, IMDB ID, TMDB ID)
    │
    ├─ Try match existing TvShow by TvDB ID (most reliable)
    │   └─ SELECT * FROM "TvShows" WHERE "TvdbId" = @tvdbId
    │
    ├─ If no match: Try match by TMDB ID
    │   └─ SELECT * FROM "TvShows" WHERE "TmdbId" = @tmdbId
    │
    ├─ If no match: Try IMDB → TMDB lookup (via TMDB API), then match by TMDB ID
    │
    ├─ If MATCH FOUND:
    │   ├─ Upsert ShowExternalId(TvShowId, Source=SuperSubtitles, ExternalId=superSubId)
    │   ├─ Backfill any missing IDs on TvShow (e.g., set TvdbId if it was null)
    │   └─ Use the EXISTING TvShow.Id for episodes/subtitles below
    │
    └─ If NO MATCH:
        ├─ Create new TvShow(Source=SuperSubtitles, TvdbId=..., TmdbId=...)
        └─ Create ShowExternalId(Source=SuperSubtitles, ExternalId=superSubId)
```

**Edge cases:**
- Show exists in Addic7ed but has no TvDB/TMDB ID yet → won't merge until `MapShowTmdbJob` populates it. After the next SuperSubtitles refresh cycle, the merge will succeed.
- Show exists only in SuperSubtitles → created as new `TvShow` with `Source = SuperSubtitles`. If Addic7ed adds it later, the Addic7ed refresh pipeline will find the existing TvDB ID and merge.

#### 2.2 Episode Merging

Episodes are matched by their **natural key**: `(TvShowId, Season, Number)`. This is already the unique index on the `Episode` table.

**Why this works with no code change:** The existing `EpisodeRepository.UpsertEpisodes()` uses `BulkMergeAsync` with `ColumnPrimaryKeyExpression = episode => new { episode.TvShowId, episode.Season, episode.Number }`. This means:
- If SuperSubtitles provides episode S01E03 for a show that already has S01E03 from Addic7ed → the existing row is reused (upserted)
- If the episode doesn't exist yet → a new row is created
- The episode's `Title` and other metadata will be updated to the latest values

**Episode upsert during SuperSubtitles refresh:**

```
For each subtitle from SuperSubtitles for a (merged) show:
    │
    ├─ Build Episode object: TvShowId = mergedShow.Id, Season = sub.Season, Number = sub.EpisodeNumber
    │
    ├─ Call EpisodeRepository.UpsertEpisodes() with the episode + its subtitles
    │   └─ BulkMerge uses (TvShowId, Season, Number) as key
    │       ├─ Existing episode? → reused, subtitles appended
    │       └─ New episode? → inserted
    │
    └─ Upsert EpisodeExternalId(EpisodeId, Source=SuperSubtitles, ExternalId=...)
```

Since the episode row is shared across providers, all its subtitles (from any source) are attached via the `EpisodeId` foreign key.

#### 2.3 Subtitle Merging

Subtitles are the leaf nodes — they are never merged across providers, only **appended**. Each subtitle row belongs to a specific provider (tracked by `Subtitle.Source`).

**Why this works with no code change:** The existing `BulkMergeAsync` for subtitles uses `ColumnPrimaryKeyExpression = subtitle => new { subtitle.DownloadUri }`. Since each provider has unique download URLs:
- Addic7ed subtitles have URIs like `https://www.addic7ed.com/...`
- SuperSubtitles subtitles have URIs like `https://feliratok.eu/...`

They never collide, so new subtitles are always **inserted** alongside existing ones.

**What happens when a user searches:**

```
GET /subtitles/get/{showUniqueId}/1/3/en

SearchSubtitlesService.FindSubtitlesAsync()
    │
    ├─ Loads the merged TvShow by UniqueId (single row, regardless of provider count)
    │
    ├─ EpisodeRepository.GetEpisodeUntrackedAsync(showId, season=1, episode=3)
    │   └─ Returns Episode with ALL subtitles (Include(e => e.Subtitles))
    │       ├─ Subtitle from Addic7ed (Source = Addic7ed, DownloadUri = addic7ed.com/...)
    │       ├─ Subtitle from Addic7ed (Source = Addic7ed, different version/language)
    │       ├─ Subtitle from SuperSubtitles (Source = SuperSubtitles, DownloadUri = feliratok.eu/...)
    │       └─ ... all subtitles for this episode, from all providers
    │
    └─ Filter by language → return combined list
```

**No changes are needed in `SearchSubtitlesService`, `SubtitlesController`, or the episode/subtitle query logic.** The database model already associates all subtitles with an episode, and episodes with a show. By merging shows and episodes at the data ingestion layer, the entire read path gets multi-provider support for free.

#### 2.4 Subtitle Download Routing

The only read-path change needed is in `SubtitleProvider.GetSubtitleFileAsync()` to route downloads to the correct upstream:

```csharp
// Current code (Addic7ed-only):
return await _addic7EdDownloader.DownloadSubtitle(creds, subtitle, token);

// New code (provider-aware):
var downloader = _registry.GetDownloader(subtitle.Source);
return await downloader.DownloadSubtitleAsync(subtitle, token);
```

- **Addic7ed subtitles**: Still use `IAddic7edDownloader` with credential rotation
- **SuperSubtitles subtitles**: Use `ISuperSubtitlesDownloader` (simple HTTP GET, no credentials)

The `Subtitle.Source` field (already present in the database) determines which downloader to use. This is the only change needed in the existing subtitle download path.

#### 2.5 Summary: What Changes vs. What Stays the Same

| Component | Changes? | Details |
|---|---|---|
| `TvShow` table | No schema change | Shows are merged into existing rows via TvDB/TMDB ID matching |
| `Episode` table | No schema change | Episodes upserted by natural key `(TvShowId, Season, Number)` |
| `Subtitle` table | No schema change | New subtitles appended with `Source = SuperSubtitles` and unique `DownloadUri` |
| `ShowExternalId` table | **New** | Maps provider-specific show IDs to merged `TvShow` rows |
| `EpisodeExternalId` table | **New** | Maps provider-specific episode IDs to merged `Episode` rows |
| `DataSource` enum | **Extended** | Add `SuperSubtitles` value |
| `SearchSubtitlesService` | No change | Already queries all subtitles for an episode regardless of source |
| `SubtitlesController` | No change | Already returns all matching subtitles |
| `EpisodeRepository` | No change | `BulkMergeAsync` already handles upserts by natural keys |
| `SubtitleProvider` | **Minor change** | Route download to correct provider via `subtitle.Source` |
| Background jobs | **New job** | `RefreshSuperSubtitlesJob` for SuperSubtitles data ingestion |

### Phase 3: Provider Abstraction Layer

#### 3.1 Generic Provider Interfaces

Create new provider-agnostic interfaces that both Addic7ed and SuperSubtitles will implement:

**ISubtitleSource** — Replaces direct dependency on `IAddic7edClient`:

```csharp
public interface ISubtitleSource
{
    DataSource Source { get; }
    IAsyncEnumerable<TvShow> GetShowsAsync(CancellationToken token);
    Task<IEnumerable<Season>> GetSeasonsAsync(TvShow show, CancellationToken token);
    Task<IEnumerable<Episode>> GetEpisodesAsync(TvShow show, int season, CancellationToken token);
}
```

**ISubtitleDownloader** — Replaces direct dependency on `IAddic7edDownloader`:

```csharp
public interface ISubtitleDownloader
{
    DataSource Source { get; }
    Task<Stream> DownloadSubtitleAsync(Subtitle subtitle, CancellationToken token);
}
```

#### 3.2 Provider Registry

A registry that maps `DataSource` → provider implementation:

```csharp
public interface ISubtitleSourceRegistry
{
    ISubtitleSource GetSource(DataSource source);
    ISubtitleDownloader GetDownloader(DataSource source);
    IEnumerable<ISubtitleSource> GetAllSources();
}
```

#### 3.3 Updated Service Layer

**SubtitleProvider** — Only change needed in the read path:
- Read `subtitle.Source` to determine which `ISubtitleDownloader` to use
- Route download to the correct provider
- Addic7ed-specific credential rotation stays isolated in the Addic7ed downloader

**ShowRefresher** — Stays Addic7ed-specific (see [Phase 4](#phase-4-background-job-pipeline) for why):
- The existing `ShowRefresher` and `RefreshAvailableShowsJob` continue to handle Addic7ed only
- SuperSubtitles gets its own dedicated refresh job that runs separately

**EpisodeRefresher** — Stays Addic7ed-specific:
- The existing `EpisodeRefresher` and `FetchSubtitlesJob` continue to handle Addic7ed episodes
- SuperSubtitles episodes/subtitles are ingested by the SuperSubtitles refresh job

### Phase 4: Background Job Pipeline

#### 4.1 Key Design Decision: Separate Jobs, Not Merged

The SuperSubtitles refresh is a **separate job** that runs **after** the Addic7ed pipeline completes. This is critical because:

1. **Addic7ed shows need TMDB/TvDB IDs first** — The existing pipeline is: `RefreshAvailableShowsJob` → `MapShowTmdbJob` → `CleanDuplicateTmdbJob` → `FetchMissingTvdbIdJob`. Only after `FetchMissingTvdbIdJob` completes do Addic7ed shows have TvDB IDs we can use for merging.
2. **SuperSubtitles provides TvDB IDs natively** — So SuperSubtitles data can be merged against Addic7ed shows once those have TvDB IDs.
3. **Different refresh cadences** — Addic7ed may need different refresh intervals than SuperSubtitles.
4. **Isolation** — If SuperSubtitles API is down, Addic7ed refresh continues unaffected.

#### 4.2 Updated Pipeline

```
RefreshAvailableShowsJob (existing, unchanged)
    │
    ├─► ShowRefresher.RefreshShowsAsync()       // Fetch Addic7ed shows
    │
    └─► ContinueJobWith: MapShowTmdbJob         // Map shows → TMDB IDs
        │
        └─► ContinueJobWith: CleanDuplicateTmdbJob
            │
            └─► ContinueJobWith: FetchMissingTvdbIdJob   // Get TvDB IDs
                │
                └─► ContinueJobWith: RefreshSuperSubtitlesJob  ◄── NEW
                    │
                    ├─► Fetch all shows from SuperSubtitles API
                    │
                    ├─► For each show:
                    │   ├─► Match to existing TvShow by TvDB/TMDB ID
                    │   ├─► Create or reuse TvShow row
                    │   ├─► Upsert ShowExternalId
                    │   ├─► For each season/episode in SubtitleCollection:
                    │   │   ├─► Build Episode objects with TvShowId = merged show
                    │   │   ├─► Build Subtitle objects with Source = SuperSubtitles
                    │   │   ├─► Call EpisodeRepository.UpsertEpisodes()
                    │   │   │   └─► BulkMerge by (TvShowId, Season, Number)
                    │   │   │       └─► Subtitles merged by DownloadUri (unique per provider)
                    │   │   └─► Upsert EpisodeExternalId
                    │   └─► Done — episode now has subtitles from both providers
                    │
                    └─► Log summary
```

#### 4.3 RefreshSuperSubtitlesJob (New)

```csharp
public class RefreshSuperSubtitlesJob
{
    // Runs AFTER FetchMissingTvdbIdJob in the pipeline
    // Also schedulable independently on its own cron

    public async Task ExecuteAsync(CancellationToken token)
    {
        // 1. Fetch all shows from SuperSubtitles API
        var allShows = await _superSubtitlesClient.GetShowList();
        
        // 2. Fetch subtitles + third-party IDs for all shows (batched)
        var showSubtitles = await _superSubtitlesClient.GetShowSubtitles(allShows);

        foreach (var showData in showSubtitles)
        {
            // 2. Try to match to existing TvShow
            var existingShow = await TryMatchShow(showData.ThirdPartyIds);

            TvShow tvShow;
            if (existingShow != null)
            {
                tvShow = existingShow;
                // Backfill missing IDs
                tvShow.TvdbId ??= showData.ThirdPartyIds.TVDBID;
            }
            else
            {
                tvShow = CreateNewTvShow(showData);
            }

            // 3. Upsert ShowExternalId
            await UpsertShowExternalId(tvShow.Id, DataSource.SuperSubtitles, showData.Show.ID);

            // 4. Build episodes + subtitles and upsert
            var episodes = BuildEpisodesFromSubtitles(tvShow, showData.SubtitleCollection);
            await _episodeRepository.UpsertEpisodes(episodes, token);

            // 5. Upsert EpisodeExternalIds
            await UpsertEpisodeExternalIds(episodes, DataSource.SuperSubtitles);
        }
    }

    private async Task<TvShow?> TryMatchShow(ThirdPartyIds ids)
    {
        if (ids.TVDBID > 0)
            return await _tvShowRepository.GetByTvdbIdAsync(ids.TVDBID).FirstOrDefaultAsync();
        if (ids.TMDBID > 0)
            return await _tvShowRepository.GetShowsByTmdbIdAsync(ids.TMDBID).FirstOrDefaultAsync();
        // IMDB fallback: lookup TMDB ID via IMDB, then match
        return null;
    }
}
```

#### 4.4 On-Demand Episode Refresh (FetchSubtitlesJob)

The existing `FetchSubtitlesJob` (triggered when a user searches and episodes are missing) stays **Addic7ed-only**. This is acceptable because:

1. SuperSubtitles data is bulk-ingested by `RefreshSuperSubtitlesJob` — all episodes/subtitles arrive in one batch
2. The Addic7ed on-demand flow is needed because Addic7ed requires per-season/per-episode queries with credentials
3. If a user searches and finds no subtitles, the Addic7ed job still fires. SuperSubtitles subtitles are either already there (from the last bulk refresh) or will arrive on the next scheduled cycle.

#### 4.5 Subtitle Download Routing

The `SubtitleProvider` determines the correct downloader based on `Subtitle.Source`:

```csharp
var downloader = _registry.GetDownloader(subtitle.Source);
return await downloader.DownloadSubtitleAsync(subtitle, token);
```

For SuperSubtitles, the downloader is a simple HTTP GET to the `DownloadUri` — no credentials, no rate limiting, no retry rotation needed.

### Phase 5: SuperSubtitles Client Module

#### 5.1 New Project: `AddictedProxy.SuperSubtitles`

```
AddictedProxy.SuperSubtitles/
├── Client/
│   ├── ISuperSubtitlesClient.cs       # Client interface
│   └── SuperSubtitlesClient.cs        # REST/gRPC client implementation
├── Service/
│   ├── SuperSubtitlesSource.cs        # Implements ISubtitleSource
│   └── SuperSubtitlesDownloader.cs    # Implements ISubtitleDownloader
├── Model/
│   └── (API response models)
└── Bootstrap/
    └── BootstrapSuperSubtitles.cs     # DI registration
```

#### 5.2 Addic7ed Adapter

Wrap existing `IAddic7edClient` and `IAddic7edDownloader` to implement the new interfaces:

```
AddictedProxy.Upstream/
├── Service/
│   ├── Addic7edSource.cs              # Implements ISubtitleSource (wraps IAddic7edClient)
│   └── Addic7edSubtitleDownloader.cs  # Implements ISubtitleDownloader (wraps IAddic7edDownloader)
```

### Phase 6: API & Frontend Updates

#### 6.1 API Changes

- **No breaking changes** to existing endpoints
- Subtitles from all providers appear in search results (unified view) — this happens automatically because the database queries already load all subtitles for an episode
- The `SubtitleDto` could optionally expose a `source` field
- Download endpoint routes to the correct provider transparently

#### 6.2 Frontend Changes

- Show search returns merged results (same show may have subtitles from multiple providers)
- Subtitle list could optionally show provider badge/icon
- No changes needed to search/download flow

## Implementation Order

### Step 1: Database Changes
1. Add `SuperSubtitles` to `DataSource` enum
2. Create `ShowExternalId` and `EpisodeExternalId` entities
3. Create EF Core migration
4. Create one-time migration to populate `ShowExternalId`/`EpisodeExternalId` from existing data

### Step 2: Provider Abstraction
1. Create `ISubtitleSource` and `ISubtitleDownloader` interfaces
2. Create `ISubtitleSourceRegistry` and implementation
3. Wrap Addic7ed services in the new interfaces
4. Update `SubtitleProvider` to route downloads via registry and `Subtitle.Source`

### Step 3: SuperSubtitles Client
1. Create `AddictedProxy.SuperSubtitles` project
2. Implement REST/gRPC client
3. Implement `ISubtitleSource` and `ISubtitleDownloader`
4. Register in DI via bootstrap

### Step 4: SuperSubtitles Refresh Job
1. Create `RefreshSuperSubtitlesJob` with show matching + episode/subtitle upsert logic
2. Chain it as a continuation of `FetchMissingTvdbIdJob` in the existing pipeline
3. Optionally add its own recurring schedule for independent refresh
4. Implement show matching by TvDB → TMDB → IMDB fallback chain
5. Reuse `EpisodeRepository.UpsertEpisodes()` for episode + subtitle ingestion

### Step 5: Testing & Validation
1. Unit tests for show matching/merging logic
2. Unit tests for episode upsert with multi-provider subtitles
3. Integration test: verify searching a merged show returns subtitles from both providers
4. Integration test: verify downloading routes to correct provider
5. Verify no regressions in existing Addic7ed-only flow

## Risks and Considerations

| Risk | Mitigation |
|---|---|
| SuperSubtitles API not yet finalized | Design abstractions that can handle REST or gRPC |
| Show merging creates duplicates | Use strict TvDB ID matching first; TMDB as fallback; never merge by name alone |
| Addic7ed shows lack TvDB IDs at merge time | SuperSubtitles refresh runs after `FetchMissingTvdbIdJob`; unmatched shows are created as new and merge on next cycle |
| Addic7ed rate limits still apply | Provider-specific credential management stays isolated in the Addic7ed downloader |
| Database migration on large dataset | Use `OneTimeMigration` with chunked processing and timeouts |
| Different subtitle metadata schemas | Normalize to common `Subtitle` model at the SuperSubtitles adapter layer |
| Episode title conflicts between providers | `BulkMergeAsync` updates to latest value; both providers' subtitles are preserved regardless |

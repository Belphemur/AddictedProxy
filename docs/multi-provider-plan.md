# Multi-Provider Architecture Plan: Adding SuperSubtitles

## Goal

Enable AddictedProxy to support multiple subtitle providers beyond Addic7ed. The first new provider is **SuperSubtitles** (based on the [SuperSubtitles](https://github.com/Belphemur/SuperSubtitles) project, which scrapes feliratok.eu). Shows from different providers should be **merged** when they represent the same media, so the rest of the app (search, browse, download) works seamlessly regardless of the subtitle source.

## SuperSubtitles Provider Overview

SuperSubtitles is a Go-based scraper for feliratok.eu (a Hungarian subtitle site). It exposes a **gRPC API** defined in [`supersubtitles.proto`](https://github.com/Belphemur/SuperSubtitles/blob/main/api/proto/v1/supersubtitles.proto) with these capabilities:

| gRPC Method | Description |
|---|---|
| **`GetShowList`** | Returns all available `Show` objects: `{ id, name, year, image_url }` |
| **`GetSubtitles`** | Returns `SubtitleCollection` for a specific show by `show_id` |
| **`GetShowSubtitles`** | Batch: accepts a list of `Show` objects, returns `ShowSubtitles` with third-party IDs and subtitle collections |
| **`CheckForUpdates`** | Checks if new subtitles are available since a given `content_id` |
| **`GetRecentSubtitles`** | Returns recently uploaded `ShowSubtitles` since a given `since_id` |
| **`DownloadSubtitle`** | Downloads a subtitle file by `download_url` + `subtitle_id`, with optional `episode` number for season pack extraction |

### Key gRPC Messages

```protobuf
message Show {
  string name = 1;
  int64 id = 2;
  int32 year = 3;
  string image_url = 4;
}

message ThirdPartyIds {
  string imdb_id = 1;
  int64 tvdb_id = 2;
  int64 tv_maze_id = 3;
  int64 trakt_id = 4;
}

message Subtitle {
  int64 id = 1;
  int64 show_id = 2;
  string show_name = 3;
  string name = 4;
  string language = 5;
  int32 season = 6;
  int32 episode = 7;
  string filename = 8;
  string download_url = 9;
  string uploader = 10;
  google.protobuf.Timestamp uploaded_at = 11;
  repeated Quality qualities = 12;
  repeated string release_groups = 13;
  string release = 14;
  bool is_season_pack = 15;
}

message ShowSubtitles {
  Show show = 1;
  ThirdPartyIds third_party_ids = 2;
  SubtitleCollection subtitle_collection = 3;
}
```

**Key differences from Addic7ed:**
- **gRPC API** with Protocol Buffers — structured, type-safe access (no HTML scraping)
- No credential/authentication system needed
- Provides third-party IDs natively (IMDB, TVDB, TVMaze, Trakt)
- Includes video quality metadata and release group information
- Supports season packs (with server-side episode extraction via `DownloadSubtitle`)
- Supports incremental updates via `CheckForUpdates` + `GetRecentSubtitles`
- **Provides season/episode data** — the `season` and `episode` fields in the `Subtitle` message are populated as `int32` values and can be used directly

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

**Show lookup via `ShowExternalId`:** The first step when processing a SuperSubtitles show is to check if we've already imported it by looking up `ShowExternalId(Source=SuperSubtitles, ExternalId=showId)`. If found, the existing `TvShow` is reused directly. If not found, we fall back to matching by third-party IDs (TvDB, then TMDB). Shows are unique per provider, so no locking is needed.

```
For each show from SuperSubtitles API:
    │
    ├─ Try lookup via ShowExternalId(Source=SuperSubtitles, ExternalId=showId)
    │   └─ If found → use the linked TvShow directly (already imported)
    │
    ├─ If not found: Extract ThirdPartyIds (TvDB ID, IMDB ID, TMDB ID)
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

**Why this works with no code change:** The existing `EpisodeRepository.UpsertEpisodes()` uses `BulkMergeAsync` with `ColumnPrimaryKeyExpression = episode => new { episode.TvShowId, episode.Season, episode.Number }`. SuperSubtitles provides `season` and `episode` as `int32` fields directly in the `Subtitle` message, so these can be used as-is. This means:
- If SuperSubtitles provides a subtitle for S01E03 of a show that already has S01E03 from Addic7ed → the existing episode row is reused (upserted)
- If the episode doesn't exist yet → a new row is created
- The episode's `Title` and other metadata will be updated to the latest values

**Episode upsert during SuperSubtitles refresh:**

```
For each subtitle from SuperSubtitles for a (merged) show:
    │
    ├─ Use subtitle.Season and subtitle.Episode directly (provided by gRPC API)
    │
    ├─ Build Episode object: TvShowId = mergedShow.Id, Season = sub.Season, Number = sub.Episode
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
| Background jobs | **New jobs** | `ImportSuperSubtitlesMigration` (one-time bulk import) + `RefreshSuperSubtitlesJob` (15-min incremental) |

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

#### 4.1 Key Design Decision: Two-Phase Ingestion (Initial Bulk + Incremental Updates)

The SuperSubtitles integration uses a **two-phase approach**:

1. **One-time bulk import** — A one-time migration job that pulls all shows and their subtitles on first run.
2. **Recurring incremental updates** — A recurring job every 15 minutes that checks for new subtitles and pulls only recent changes.

This design is critical because:

1. **Addic7ed shows need TMDB/TvDB IDs first** — The existing pipeline is: `RefreshAvailableShowsJob` → `MapShowTmdbJob` → `CleanDuplicateTmdbJob` → `FetchMissingTvdbIdJob`. Only after `FetchMissingTvdbIdJob` completes do Addic7ed shows have TvDB IDs we can use for merging.
2. **SuperSubtitles provides TvDB IDs natively** — So SuperSubtitles data can be merged against Addic7ed shows once those have TvDB IDs.
3. **Incremental updates are efficient** — After the initial bulk import, only new subtitles are fetched every 15 minutes using the max external subtitle ID as a cursor.
4. **Season packs are ignored** — Subtitles where `is_season_pack = true` are skipped during ingestion. Only individual episode subtitles are imported.
5. **Isolation** — If SuperSubtitles gRPC API is down, Addic7ed refresh continues unaffected.
6. **Rate limiting protection (bulk import only)** — The one-time bulk import must avoid overwhelming the SuperSubtitles gRPC server (which scrapes feliratok.eu under the hood). The show list is split into batches with a **configurable delay between batches**. The recurring 15-minute incremental updates do **not** need batch delays because they only fetch a small number of recent subtitles.

#### 4.2 Phase A: One-Time Bulk Import Job

This job runs once (via the `OneTimeMigration` framework) to populate the database with all existing SuperSubtitles data.

**Rate limiting:** The show list is split into batches (configurable, e.g. 10 shows per batch). After each `GetShowSubtitles` gRPC call, the job waits for a configurable delay (e.g. 2–5 seconds) before sending the next batch. This prevents overwhelming the upstream SuperSubtitles server, which itself scrapes feliratok.eu.

```
ImportSuperSubtitlesMigration (OneTimeMigration, runs once after FetchMissingTvdbIdJob)
    │
    ├─► Step 1: GetShowList() via gRPC
    │   └─► Returns all Show objects (id, name, year, image_url)
    │
    ├─► Step 2: Split shows into batches of N (configurable batch size)
    │
    ├─► Step 3: For each batch:
    │   ├─► GetShowSubtitles(batch) via gRPC
    │   │   └─► Returns ShowSubtitles[] with ThirdPartyIds + SubtitleCollection
    │   │
    │   ├─► For each ShowSubtitles in batch response:
    │   │   ├─► Lookup ShowExternalId(Source=SuperSubtitles, ExternalId=showId)
    │   │   ├─► If not found: match by TvDB/TMDB ID or create new TvShow
    │   │   ├─► Upsert ShowExternalId(Source=SuperSubtitles)
    │   │   │
    │   │   ├─► Filter out subtitles where is_season_pack = true
    │   │   │
    │   │   ├─► For each remaining subtitle:
    │   │   │   ├─► Use subtitle.Season and subtitle.Episode directly
    │   │   │   ├─► Build Episode(TvShowId, Season, Number)
    │   │   │   ├─► Build Subtitle(Source=SuperSubtitles, DownloadUri=download_url)
    │   │   │   └─► Track max subtitle ID for use as incremental cursor
    │   │   │
    │   │   ├─► Call EpisodeRepository.UpsertEpisodes()
    │   │   │   └─► BulkMerge by (TvShowId, Season, Number)
    │   │   │       └─► Subtitles merged by DownloadUri (unique per provider)
    │   │   │
    │   │   └─► Upsert EpisodeExternalId(Source=SuperSubtitles)
    │   │
    │   └─► ⏳ Wait for configurable delay before next batch (rate limiting)
    │
    └─► Store the max SuperSubtitles subtitle ID for incremental updates
```

**Registration as a one-time migration:**

```csharp
[MigrationDate(2026, 2, 10)]
public class ImportSuperSubtitlesMigration : IMigration
{
    private readonly ISuperSubtitlesClient _superSubtitlesClient;
    private readonly SuperSubtitlesImportConfig _config;
    // ... other dependencies

    public async Task ExecuteAsync(CancellationToken token)
    {
        // 1. Call GetShowList() via gRPC
        var allShows = await _superSubtitlesClient.GetShowListAsync(token);

        // 2. Split into batches to avoid rate limiting
        var batches = allShows.Chunk(_config.BatchSize); // e.g. 10 shows per batch

        long maxSubtitleId = 0;
        foreach (var batch in batches)
        {
            // 3. Call GetShowSubtitles for this batch via gRPC
            var showSubtitles = await _superSubtitlesClient
                .GetShowSubtitlesAsync(batch, token);

            foreach (var showData in showSubtitles)
            {
                // 4. Lookup via ShowExternalId first, then fall back to TvDB/TMDB matching
                var tvShow = await MatchOrCreateShow(showData);
                await UpsertShowExternalId(tvShow, showData.Show.Id);

                // 5. Filter out season packs
                var nonPackSubtitles = showData.SubtitleCollection.Subtitles
                    .Where(s => !s.IsSeasonPack);

                // 6. Build episodes + subtitles using season/episode from proto directly
                var episodes = BuildEpisodesFromSubtitles(tvShow, nonPackSubtitles);
                await _episodeRepository.UpsertEpisodes(episodes, token);

                // 7. Track max ID
                maxSubtitleId = Math.Max(maxSubtitleId,
                    nonPackSubtitles.Max(s => s.Id));
            }

            // 8. Wait between batches to avoid rate limiting
            await Task.Delay(_config.DelayBetweenBatches, token); // e.g. 2-5 seconds
        }

        // 9. Store max ID for incremental updates
        await _superSubtitlesStateRepository.SetMaxSubtitleIdAsync(maxSubtitleId);
    }

    private async Task<TvShow?> MatchOrCreateShow(ShowSubtitles showData)
    {
        // 1. Check ShowExternalId first (fast path for already-imported shows)
        var existingByExtId = await _showExternalIdRepository
            .GetByExternalIdAsync(DataSource.SuperSubtitles, showData.Show.Id.ToString());
        if (existingByExtId != null)
            return existingByExtId.TvShow;

        // 2. Fall back to TvDB/TMDB matching for first-time merge
        var ids = showData.ThirdPartyIds;
        if (ids.TvdbId > 0)
        {
            var match = await _tvShowRepository.GetByTvdbIdAsync(ids.TvdbId);
            if (match != null) return match;
        }
        if (ids.ImdbId is not null)
        {
            // IMDB → TMDB lookup, then match
        }

        // 3. No match — create new TvShow
        return CreateNewTvShow(showData);
    }
}
```

**Configuration:**

```csharp
public class SuperSubtitlesImportConfig
{
    /// <summary>Number of shows to request per gRPC batch call.</summary>
    public int BatchSize { get; set; } = 10;

    /// <summary>Delay between batch calls to avoid upstream rate limiting.</summary>
    public TimeSpan DelayBetweenBatches { get; set; } = TimeSpan.FromSeconds(3);
}
```

```json
{
    "SuperSubtitles": {
        "GrpcAddress": "http://supersubtitles:8080",
        "Import": {
            "BatchSize": 10,
            "DelayBetweenBatches": "00:00:03"
        }
    }
}
```

#### 4.3 Phase B: Recurring Incremental Update Job (Every 15 Minutes)

After the bulk import, a recurring Hangfire job runs every 15 minutes to pull only new subtitles.

```
RefreshSuperSubtitlesJob (Recurring, every 15 minutes)
    │
    ├─► Step 1: Load max SuperSubtitles subtitle ID from local state
    │
    ├─► Step 2: CheckForUpdates(content_id) via gRPC
    │   └─► Returns { has_updates, series_count, film_count }
    │
    ├─► Step 3: If has_updates = false → exit early (nothing to do)
    │
    ├─► Step 4: GetRecentSubtitles(since_id = maxSubtitleId) via gRPC
    │   └─► Returns ShowSubtitles[] with only new/updated subtitles
    │
    ├─► Step 5: For each ShowSubtitles in response:
    │   ├─► Lookup ShowExternalId(Source=SuperSubtitles, ExternalId=showId)
    │   ├─► If not found: match by TvDB/TMDB ID or create new TvShow
    │   ├─► Upsert ShowExternalId(Source=SuperSubtitles)
    │   │
    │   ├─► Filter out subtitles where is_season_pack = true
    │   │
    │   ├─► For each remaining subtitle:
    │   │   ├─► Use subtitle.Season and subtitle.Episode directly
    │   │   ├─► Build Episode + Subtitle objects
    │   │   └─► Track new max subtitle ID
    │   │
    │   ├─► Call EpisodeRepository.UpsertEpisodes()
    │   └─► Upsert EpisodeExternalId(Source=SuperSubtitles)
    │
    └─► Update stored max subtitle ID
```

**Implementation:**

```csharp
public class RefreshSuperSubtitlesJob
{
    // Scheduled as recurring job: every 15 minutes
    // Cron: "*/15 * * * *"

    [MaxConcurrency(1)]
    public async Task ExecuteAsync(CancellationToken token)
    {
        // 1. Load current cursor (max subtitle ID from SuperSubtitles)
        var maxId = await _stateRepository.GetMaxSubtitleIdAsync();

        // 2. Check if there are updates
        var updateCheck = await _superSubtitlesClient.CheckForUpdatesAsync(
            maxId.ToString(), token);

        if (!updateCheck.HasUpdates)
            return; // Nothing new

        // 3. Fetch recent subtitles since our last known ID
        var recentSubtitles = await _superSubtitlesClient
            .GetRecentSubtitlesAsync(maxId, token);

        long newMaxId = maxId;
        foreach (var showData in recentSubtitles)
        {
            // 4. Lookup via ShowExternalId first, then fall back to TvDB/TMDB matching
            var tvShow = await MatchOrCreateShow(showData);
            await UpsertShowExternalId(tvShow, showData.Show.Id);

            // 5. Filter out season packs
            var nonPackSubtitles = showData.SubtitleCollection.Subtitles
                .Where(s => !s.IsSeasonPack);

            // 6. Build episodes + subtitles using season/episode from proto directly
            var episodes = BuildEpisodesFromSubtitles(tvShow, nonPackSubtitles);
            await _episodeRepository.UpsertEpisodes(episodes, token);

            // 7. Track new max ID
            newMaxId = Math.Max(newMaxId,
                nonPackSubtitles.DefaultIfEmpty().Max(s => s?.Id ?? 0));
        }

        // 8. Persist updated cursor
        await _stateRepository.SetMaxSubtitleIdAsync(newMaxId);
    }
}
```

#### 4.4 Updated Full Pipeline (Addic7ed + SuperSubtitles)

```
┌─────────────────────────────────────────────────────────────────────┐
│                     ADDIC7ED PIPELINE (existing)                   │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  RefreshAvailableShowsJob (recurring)                               │
│      │                                                              │
│      ├─► ShowRefresher.RefreshShowsAsync()                          │
│      │                                                              │
│      └─► ContinueJobWith: MapShowTmdbJob                            │
│              └─► ContinueJobWith: CleanDuplicateTmdbJob             │
│                      └─► ContinueJobWith: FetchMissingTvdbIdJob     │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                 SUPERSUBTITLES PIPELINE (new)                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ImportSuperSubtitlesMigration (one-time, via OneTimeMigration)     │
│      └─► Bulk import: GetShowList → GetShowSubtitles → merge all   │
│                                                                     │
│  RefreshSuperSubtitlesJob (recurring every 15 minutes)             │
│      └─► Incremental: CheckForUpdates → GetRecentSubtitles → merge │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

The two pipelines are **independent** — SuperSubtitles does not chain off the Addic7ed pipeline. The bulk import runs once as a migration, and the incremental job runs on its own 15-minute schedule.

#### 4.5 On-Demand Episode Refresh (FetchSubtitlesJob)

The existing `FetchSubtitlesJob` (triggered when a user searches and episodes are missing) stays **Addic7ed-only**. This is acceptable because:

1. SuperSubtitles data is bulk-ingested by the one-time migration, then kept fresh via the 15-minute recurring job
2. The Addic7ed on-demand flow is needed because Addic7ed requires per-season/per-episode queries with credentials
3. If a user searches and finds no subtitles, the Addic7ed job still fires. SuperSubtitles subtitles are either already there (from the last refresh) or will arrive within the next 15-minute cycle.

#### 4.6 Subtitle Download Routing

The `SubtitleProvider` determines the correct downloader based on `Subtitle.Source`:

```csharp
var downloader = _registry.GetDownloader(subtitle.Source);
return await downloader.DownloadSubtitleAsync(subtitle, token);
```

For SuperSubtitles, the downloader uses the **`DownloadSubtitle` gRPC method** instead of a simple HTTP GET:

```csharp
public class SuperSubtitlesDownloader : ISubtitleDownloader
{
    public DataSource Source => DataSource.SuperSubtitles;

    public async Task<Stream> DownloadSubtitleAsync(Subtitle subtitle, CancellationToken token)
    {
        // Use the gRPC DownloadSubtitle method
        // The episode field extracts a specific episode from season packs (0 = full file)
        var response = await _grpcClient.DownloadSubtitleAsync(new DownloadSubtitleRequest
        {
            DownloadUrl = subtitle.DownloadUri,
            SubtitleId = subtitle.ExternalId,
            Episode = subtitle.Number // Extract specific episode if from season pack
        }, token);

        return new MemoryStream(response.Content.ToByteArray());
    }
}
```

No credentials, no rate limiting, no retry rotation needed — the SuperSubtitles gRPC API handles downloads directly.

### Phase 5: SuperSubtitles Client Module

#### 5.1 New Project: `AddictedProxy.SuperSubtitles`

```
AddictedProxy.SuperSubtitles/
├── Client/
│   ├── ISuperSubtitlesClient.cs       # Client interface
│   └── SuperSubtitlesGrpcClient.cs    # gRPC client implementation (wraps generated stubs)
├── Service/
│   ├── SuperSubtitlesSource.cs        # Implements ISubtitleSource
│   └── SuperSubtitlesDownloader.cs    # Implements ISubtitleDownloader (uses gRPC DownloadSubtitle)
├── State/
│   ├── ISuperSubtitlesStateRepository.cs  # Tracks max subtitle ID for incremental updates
│   └── SuperSubtitlesStateRepository.cs
├── Proto/
│   └── (generated gRPC stubs from supersubtitles.proto)
├── Model/
│   └── (mapping helpers between gRPC messages and domain entities)
└── Bootstrap/
    └── BootstrapSuperSubtitles.cs     # DI registration (gRPC channel, client, services)
```

#### 5.2 gRPC Client Interface

```csharp
public interface ISuperSubtitlesClient
{
    /// <summary>Fetches all available shows.</summary>
    Task<IReadOnlyList<Show>> GetShowListAsync(CancellationToken token);

    /// <summary>Fetches shows with their subtitles and third-party IDs (batch).</summary>
    Task<IReadOnlyList<ShowSubtitles>> GetShowSubtitlesAsync(
        IEnumerable<Show> shows, CancellationToken token);

    /// <summary>Checks if new subtitles are available since a given content ID.</summary>
    Task<CheckForUpdatesResponse> CheckForUpdatesAsync(
        string contentId, CancellationToken token);

    /// <summary>Fetches recently uploaded subtitles since a given subtitle ID.</summary>
    Task<IReadOnlyList<ShowSubtitles>> GetRecentSubtitlesAsync(
        long sinceId, CancellationToken token);

    /// <summary>Downloads a subtitle file via gRPC.</summary>
    Task<DownloadSubtitleResponse> DownloadSubtitleAsync(
        string downloadUrl, string subtitleId, int episode, CancellationToken token);
}
```

#### 5.3 gRPC Channel Configuration

```csharp
public class BootstrapSuperSubtitles : IBootstrap
{
    public void ConfigureServices(IServiceCollection services,
        IConfiguration configuration, ILoggingBuilder logging)
    {
        // Register gRPC channel to SuperSubtitles server
        services.AddGrpcClient<SuperSubtitlesService.SuperSubtitlesServiceClient>(options =>
        {
            options.Address = new Uri(configuration["SuperSubtitles:GrpcAddress"]!);
        });

        services.AddScoped<ISuperSubtitlesClient, SuperSubtitlesGrpcClient>();
        services.AddScoped<ISuperSubtitlesStateRepository, SuperSubtitlesStateRepository>();
        services.AddScoped<ISubtitleDownloader, SuperSubtitlesDownloader>();
    }
}
```

#### 5.4 Addic7ed Adapter

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
2. Add `supersubtitles.proto` and generate gRPC stubs
3. Implement gRPC client wrapper (`SuperSubtitlesGrpcClient`)
4. Implement `ISubtitleSource` and `ISubtitleDownloader` (using gRPC `DownloadSubtitle`)
5. Register in DI via bootstrap (gRPC channel, client, services)

### Step 4: SuperSubtitles Import & Refresh Jobs
1. Create `ImportSuperSubtitlesMigration` (one-time bulk import via `OneTimeMigration` framework)
   - Fetch all shows via `GetShowList`, split into batches
   - For each batch: call `GetShowSubtitles`, process results, **wait between batches** to avoid rate limiting
   - Filter out season packs (`is_season_pack = true`)
   - Use `ShowExternalId` for fast lookup of already-imported shows, fall back to TvDB → TMDB → IMDB matching for first-time merge
   - Use season/episode fields directly from the proto `Subtitle` message
   - Reuse `EpisodeRepository.UpsertEpisodes()` for episode + subtitle ingestion
   - Store max subtitle ID as cursor for incremental updates
2. Create `RefreshSuperSubtitlesJob` (recurring every 15 minutes)
   - Check for updates via `CheckForUpdates` using stored max subtitle ID
   - If updates exist, call `GetRecentSubtitles` to fetch only new data
   - Process and merge new subtitles (same logic as bulk import, no batch delays needed — dataset is small)
3. Add `SuperSubtitlesImportConfig` for configurable batch size and delay between batches (bulk import only)

### Step 5: Testing & Validation
1. Unit tests for show matching/merging logic (ShowExternalId lookup + TvDB/TMDB fallback)
2. Unit tests for episode upsert with multi-provider subtitles
3. Integration test: verify searching a merged show returns subtitles from both providers
4. Integration test: verify downloading routes to correct provider
5. Verify no regressions in existing Addic7ed-only flow

## Risks and Considerations

| Risk | Mitigation |
|---|---|
| SuperSubtitles upstream rate limiting | Bulk import batches gRPC calls with configurable delays between batches (e.g. 3s) and processes shows in chunks of ~10; incremental 15-min updates don't need delays since the recent data is small |
| Show merging creates duplicates | Use `ShowExternalId` lookup first for fast deduplication of already-imported shows; fall back to strict TvDB ID matching, then TMDB; never merge by name alone |
| Addic7ed shows lack TvDB IDs at merge time | SuperSubtitles refresh runs independently; unmatched shows are created as new and merge on next cycle |
| Addic7ed rate limits still apply | Provider-specific credential management stays isolated in the Addic7ed downloader |
| Database migration on large dataset | Use `OneTimeMigration` with chunked/batched processing, configurable delays, and timeouts |
| Different subtitle metadata schemas | Normalize to common `Subtitle` model at the SuperSubtitles adapter layer |
| Episode title conflicts between providers | `BulkMergeAsync` updates to latest value; both providers' subtitles are preserved regardless |

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

#### 1.4 Show Merging Strategy

Shows from different providers that represent the same media need to be merged. The primary merge key is **TvDB ID** (available from both providers).

**Merge Logic:**
1. When a new show arrives from SuperSubtitles with a TvDB ID
2. Check if a show with that TvDB ID already exists (from Addic7ed)
3. If yes: Add a `ShowExternalId` entry for the new provider to the existing show
4. If no: Create a new `TvShow` with `Source = SuperSubtitles`

**Fallback merge keys** (in priority order):
1. TvDB ID (most reliable)
2. TMDB ID
3. IMDB ID (via TMDB lookup)
4. Name matching (fuzzy, least reliable)

#### 1.5 One-Time Data Migration

Use the `OneTimeMigration` framework to migrate existing data:

```csharp
[MigrationDate(2026, 2, 6)]
public class MigrateExternalIdsToNewTableMigration : IMigration
{
    // 1. Copy TvShow.ExternalId → ShowExternalId (Source = Addic7ed)
    // 2. Copy Episode.ExternalId → EpisodeExternalId (Source = Addic7ed)
}
```

### Phase 2: Provider Abstraction Layer

#### 2.1 Generic Provider Interfaces

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

#### 2.2 Provider Registry

A registry that maps `DataSource` → provider implementation:

```csharp
public interface ISubtitleSourceRegistry
{
    ISubtitleSource GetSource(DataSource source);
    ISubtitleDownloader GetDownloader(DataSource source);
    IEnumerable<ISubtitleSource> GetAllSources();
}
```

#### 2.3 Updated Service Layer

**ShowRefresher** changes:
- Instead of calling `IAddic7edClient` directly, iterate over all `ISubtitleSource` implementations
- Merge shows using TvDB/TMDB IDs
- Store `ShowExternalId` entries for each provider

**SubtitleProvider** changes:
- Read `subtitle.Source` to determine which `ISubtitleDownloader` to use
- Route download to the correct provider

**EpisodeRefresher** changes:
- Accept provider context to know which source to query
- Store `EpisodeExternalId` entries

### Phase 3: SuperSubtitles Client Module

#### 3.1 New Project: `AddictedProxy.SuperSubtitles`

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

#### 3.2 Addic7ed Adapter

Wrap existing `IAddic7edClient` and `IAddic7edDownloader` to implement the new interfaces:

```
AddictedProxy.Upstream/
├── Service/
│   ├── Addic7edSource.cs              # Implements ISubtitleSource (wraps IAddic7edClient)
│   └── Addic7edSubtitleDownloader.cs  # Implements ISubtitleDownloader (wraps IAddic7edDownloader)
```

### Phase 4: Background Job Updates

#### 4.1 Multi-Provider Show Refresh

```
RefreshAllProvidersJob (new)
    │
    ├─► For each ISubtitleSource:
    │   ├─► Fetch shows
    │   ├─► Merge with existing (by TvDB/TMDB ID)
    │   └─► Upsert ShowExternalId entries
    │
    └─► ContinueJobWith: MapShowTmdbJob, etc.
```

#### 4.2 Episode Refresh

The `FetchSubtitlesJob` needs to know which provider(s) to query:
- If the show has a `ShowExternalId` for a given provider, query that provider
- Query all available providers for the show to maximize subtitle coverage

#### 4.3 Subtitle Download Routing

The `SubtitleProvider` determines the correct downloader based on `Subtitle.Source`:

```csharp
var downloader = _registry.GetDownloader(subtitle.Source);
return await downloader.DownloadSubtitleAsync(subtitle, token);
```

### Phase 5: API & Frontend Updates

#### 5.1 API Changes

- **No breaking changes** to existing endpoints
- Subtitles from all providers appear in search results (unified view)
- The `SubtitleDto` could optionally expose a `source` field
- Download endpoint routes to the correct provider transparently

#### 5.2 Frontend Changes

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
4. Update `ShowRefresher`, `SeasonRefresher`, `EpisodeRefresher` to use registry
5. Update `SubtitleProvider` to route downloads via registry

### Step 3: SuperSubtitles Client
1. Create `AddictedProxy.SuperSubtitles` project
2. Implement REST/gRPC client
3. Implement `ISubtitleSource` and `ISubtitleDownloader`
4. Register in DI via bootstrap

### Step 4: Show Merging
1. Implement merge logic in `ShowRefresher` using TvDB/TMDB/IMDB IDs
2. Handle edge cases (no external IDs, name-only matching)
3. Add background job for periodic merge reconciliation

### Step 5: Job Updates
1. Update `RefreshAvailableShowsJob` to iterate providers
2. Update `FetchSubtitlesJob` to query all relevant providers
3. Add SuperSubtitles-specific refresh scheduling

### Step 6: Testing & Validation
1. Unit tests for show merging logic
2. Integration tests for multi-provider search
3. Verify download routing works correctly

## Risks and Considerations

| Risk | Mitigation |
|---|---|
| SuperSubtitles API not yet finalized | Design abstractions that can handle REST or gRPC |
| Show merging creates duplicates | Use strict TvDB ID matching first, flag uncertain merges |
| Addic7ed rate limits still apply | Provider-specific credential management stays isolated |
| Database migration on large dataset | Use `OneTimeMigration` with chunked processing and timeouts |
| Different subtitle metadata schemas | Normalize to common model at the provider adapter layer |

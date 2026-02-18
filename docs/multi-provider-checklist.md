# Multi-Provider Implementation Checklist

Progress tracker for the [Multi-Provider Architecture Plan](multi-provider-plan.md).

## Phase 1: Database Schema Changes

- [x] Extend `DataSource` enum with `SuperSubtitles` value
- [x] Create `ShowExternalId` entity (PK, TvShowId FK, Source, ExternalId, unique constraints)
- [x] Create `EpisodeExternalId` entity (PK, EpisodeId FK, Source, ExternalId, unique constraints)
- [x] Create `SeasonPackSubtitle` entity (full schema with indexes)
- [x] Add navigation properties to `TvShow` and `Episode` for new entities
- [x] Update `EntityContext` with new `DbSet`s and model configuration
- [x] Remove unique index on `TvShow.ExternalId` (replaced by `ShowExternalId` table)
- [x] Generate EF Core migration (`AddMultiProviderTables`)
- [x] Create `IShowExternalIdRepository` / `ShowExternalIdRepository` with bulk upsert
- [x] Create `IEpisodeExternalIdRepository` / `EpisodeExternalIdRepository` with bulk upsert
- [x] Create `ISeasonPackSubtitleRepository` / `SeasonPackSubtitleRepository` with bulk upsert
- [x] Register new repositories in `BootstrapDatabase` DI
- [x] Create one-time migration (`MigrateExternalIdsToNewTableMigration`) to populate `ShowExternalId`/`EpisodeExternalId` from existing `TvShow.ExternalId` and `Episode.ExternalId`
- [x] Add `ExternalId` column to `Subtitle` entity with `(Source, ExternalId)` unique index
- [x] Generate EF Core migration (`AddExternalIdToSubtitle`)
- [x] Create one-time migration (`MigrateSubtitleExternalIdMigration`) to populate `Subtitle.ExternalId` from `DownloadUri`
- [x] Update `BulkMergeAsync` to ignore `ExternalId` on merge update
- [x] Register migrations in `BootstrapMigration`

## Phase 2: Data Merging Strategy

- [x] Implement show merging logic (lookup `ShowExternalId` → fallback TvDB → TMDB → create new)
- [x] Implement episode merging via natural key `(TvShowId, Season, Number)` using `EpisodeRepository.UpsertEpisodes()`
- [x] Implement subtitle appending (insert with `Source = SuperSubtitles`, unique by `DownloadUri`)
- [x] Implement season pack ingestion (store `is_season_pack = true` subtitles in `SeasonPackSubtitle`)
- [x] Handle edge cases (missing TvDB/TMDB IDs, shows only in one provider)

## Phase 3: Provider Abstraction Layer

- [x] Create `ISubtitleSource` interface
- [x] Create `ISubtitleDownloader` interface (extends `IEnumService<DataSource>`)
- [x] Create `SubtitleDownloaderFactory` (extends `EnumFactory<DataSource, ISubtitleDownloader>`)
- [x] Implement `Addic7edSubtitleDownloader` (wraps `IAddic7edDownloader` + `ICredentialsService` with retry/rotation)
- [x] Implement `SuperSubtitlesSubtitleDownloader` (uses gRPC `DownloadSubtitle` via `subtitle.ExternalId`)
- [x] Update `SubtitleProvider` to route downloads via `SubtitleDownloaderFactory` and `Subtitle.Source`
- [x] Register downloaders and factory in `BootstrapProvider`
- [x] Create internal `IProviderShowRefresher`, `IProviderSeasonRefresher`, `IProviderEpisodeRefresher` interfaces extending `IEnumService<DataSource>`
- [x] Extract Addic7ed-specific logic into `Addic7edShowRefresher` (`IProviderShowRefresher`), create no-op `SuperSubtitlesShowRefresher`
- [x] Extract Addic7ed-specific logic into `Addic7edSeasonRefresher` (`IProviderSeasonRefresher`), create no-op `SuperSubtitlesSeasonRefresher`
- [x] Extract Addic7ed-specific logic into `Addic7edEpisodeRefresher` (`IProviderEpisodeRefresher`), create no-op `SuperSubtitlesEpisodeRefresher`
- [x] Update `ShowRefresher` to route via `ShowExternalId` lookup + `EnumFactory<DataSource, IProviderShowRefresher>` (keep `IShowRefresher` interface unchanged)
- [x] Update `SeasonRefresher` to route via `ShowExternalId` lookup + `EnumFactory<DataSource, IProviderSeasonRefresher>` (keep `ISeasonRefresher` interface unchanged)
- [x] Update `EpisodeRefresher` to route via `ShowExternalId` lookup + `EnumFactory<DataSource, IProviderEpisodeRefresher>` (keep `IEpisodeRefresher` interface unchanged, pass `ShowExternalId` to provider impls)
- [x] Register provider-specific implementations in `BootstrapProvider` DI (auto-discovered via `IEnumService<DataSource>`)
- [x] No changes needed to callers — they continue using `IShowRefresher` / `ISeasonRefresher` / `IEpisodeRefresher`

## Phase 4: Background Job Pipeline

### Phase 4A: One-Time Bulk Import

- [x] Create `SuperSubtitlesImportConfig` (batch size, min/max delay)
- [x] Add config section to `appsettings.json`
- [x] Create `ISuperSubtitlesStateRepository` / `SuperSubtitlesStateRepository` (tracks max subtitle ID cursor)
- [x] Create `ImportSuperSubtitlesJob` (one-time Hangfire fire-and-forget job, enqueued at startup)
  - [x] Idempotent: skips if max subtitle ID cursor already exists
  - [x] Fetch all shows via `GetShowList` (streamed), collect into batches
  - [x] For each batch: call `GetShowSubtitles`, process stream asynchronously
  - [x] Wrap each show's data in a database transaction via `ITransactionManager<EntityContext>`
  - [x] Process `ShowInfo` items (match/create shows, upsert `ShowExternalId`)
  - [x] Process `Subtitle` items (upsert episodes + subtitles, or store season packs)
  - [x] Ensure `Season` entities exist before upserting episodes
  - [x] Rate-limit with configurable delay between batches
  - [x] Track progress via Hangfire.Console (`PerformContext`, progress bar, console lines)
  - [x] Store max subtitle ID as cursor for incremental updates

### Phase 4B: Recurring Incremental Updates

- [ ] Create `RefreshSuperSubtitlesJob` (recurring every 15 minutes)
  - [ ] Load max subtitle ID from state repository
  - [ ] `CheckForUpdates` → early exit if no updates
  - [ ] `GetRecentSubtitles(since_id)` → process stream (same logic as bulk import)
  - [ ] Wrap each show's data in a database transaction
  - [ ] Update stored max subtitle ID
- [ ] Register recurring job in Hangfire

## Phase 5: SuperSubtitles Client Module

- [x] Create `SuperSubtitleClient` project and add to solution
- [x] Add `supersubtitles.proto` and configure gRPC code generation (`Grpc.Tools`)
- [x] Add `Google.Protobuf`, `Grpc.Net.ClientFactory`, `Grpc.Tools` to `Directory.Packages.props`
- [x] Create `SuperSubtitlesConfig` (bound from `"SuperSubtitles"` config section via `IOptions<T>`)
- [x] Create `ISuperSubtitlesClient` interface (all 6 RPCs, `IAsyncEnumerable<T>` for streaming)
- [x] Create `SuperSubtitlesClientImpl` (wraps generated gRPC client, converts streams to `IAsyncEnumerable<T>`)
- [x] Create `BootstrapSuperSubtitles` (DI: options binding, `AddGrpcClient` with standard resilience, register `ISuperSubtitlesClient`)
- [x] Add project reference to `AddictedProxy.csproj`
- [x] Add `BootstrapSuperSubtitles` assembly to `Program.cs`
- [x] Add `SuperSubtitles` config section to `appsettings.json` and `appsettings.Development.json`
- [x] Implement `SuperSubtitlesSource` (`ISubtitleSource`)

## Phase 6: API & Frontend Updates

- [ ] Optionally expose `source` field in `SubtitleDto`
- [ ] Optionally add provider badge/icon in frontend subtitle list
- [ ] Verify no breaking changes to existing endpoints

## Infrastructure

- [x] Add SuperSubtitles service to `compose.yaml`
- [x] Add Hangfire.Console package for job progress tracking
- [x] Update multi-provider plan documentation

## Testing & Validation

- [x] Unit tests for show matching/merging logic
- [x] Unit tests for episode upsert with multi-provider subtitles
- [ ] Integration test: search merged show returns subtitles from both providers
- [ ] Integration test: download routes to correct provider
- [ ] Verify no regressions in existing Addic7ed-only flow

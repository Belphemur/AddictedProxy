# Background Jobs

## Overview

AddictedProxy uses **Hangfire** with PostgreSQL storage for background job processing. Jobs handle show/episode refresh, subtitle storage, TMDB mapping, and one-time data migrations. Each job type runs on a dedicated queue with configurable concurrency.

**Hangfire.Console** is integrated to provide real-time progress tracking and console output within the Hangfire Dashboard for all background jobs.

## Job Infrastructure

### Queues and Concurrency

A single Hangfire server processes all queues with `WorkerCount = Environment.ProcessorCount * 6`; per-job concurrency is limited with the `[MaximumConcurrentExecutions]` attribute:

| Queue | Job | Max Concurrency | Purpose |
|-------|-----|-----------------|---------|
| `default` | Various | Default | General purpose |
| `fetch-subtitles` | FetchSubtitlesJob | 10 | Fetch episodes/subtitles from upstream |
| `refresh-one-show` | RefreshSingleShowJob | 6 | Refresh single show metadata |
| `store-subtitle` | StoreSubtitleJob, StoreSeasonPackJob, MigrateCompressionJob | Default | Store subtitle/season-pack files to S3 |
| `download-creds-checker` | ResetDownloadCredJob | Default | Reset download-exceeded credentials |

### Job Attributes

Custom Hangfire attributes used:
- **`[UniqueJob(Order, TtlFingerprintSeconds)]`**: Prevents duplicate jobs (deduplication by parameters, with an optional fingerprint TTL)
- **`[MaximumConcurrentExecutions(maxConcurrency, queueSize)]`**: Limits concurrent execution (from the `Hangfire.MaximumConcurrentExecutions` package)
- **`[AutomaticRetry(Attempts, OnAttemptsExceeded, DelaysInSeconds)]`**: Automatic retry with explicit delay steps (exponential-style backoff)
- **`[Queue(name)]`**: Assigns the job to a queue

### Progress Tracking

Hangfire.Console provides:
- **Progress bars**: `IProgressBar progressBar = context.WriteProgressBar();`
- **Console output**: `context.WriteLine("message");`
- **Time series data**: Real-time updates visible in Hangfire Dashboard
- **Structured logging**: Color-coded output (Info, Warning, Error)

Jobs receive `PerformContext` as a parameter to access console features:

```csharp
public async Task Execute(PerformContext context)
{
    var progress = context.WriteProgressBar();
    progress.SetValue(50); // 50% complete
    context.WriteLine("Processing item...");
}
```

## Show Refresh Jobs

### RefreshAvailableShowsJob

**Trigger**: Scheduled recurring job (Hangfire cron)  
**Purpose**: Refreshes the entire show catalog from Addic7ed  
**Pipeline**: Creates a chain of continuation jobs:

```
RefreshAvailableShowsJob                       // Recurring: "0 2 * * *" (daily at 02:00)
    │
    ├─► ShowRefresher.RefreshShowsAsync()     // Fetch all shows from Addic7ed
    │
    └─► Enqueue MapShowTmdbJob                // Map shows to TMDB
            └─► ContinueJobWith CleanDuplicateTmdbJob   // Clean duplicate TMDB mappings
                    └─► ContinueJobWith FetchMissingTvdbIdJob   // Fetch missing TvDB IDs
```

### RefreshSingleShowJob

**Trigger**: User-initiated via `POST /shows/{showId}/refresh`  
**Queue**: `refresh-one-show`  
**Concurrency**: Max 6 concurrent  
**Retry**: 20 attempts, exponential backoff  
**TTL**: 2 hours (unique job)  
**Timeout**: 10 minutes  

**Behavior**:
1. Acquires async keyed lock per show ID
2. Delegates to `ShowRefresher.RefreshShowAsync()`
3. Refreshes seasons → episodes → subtitles for the show
4. Enqueues a `CleanupEmptySeasonsJob` continuation for the show

### CheckCompletedTmdbJob

**Trigger**: Two recurring jobs scheduled by `SchedulerHostedService`:
- `{name}-not-completed` daily at 02:50 (`50 2 * * *`) for shows not yet marked completed
- `{name}-completed` every 2 days at 02:10 (`10 2 * * */2`) for completed shows

**Purpose**: Re-checks against TMDB whether shows are completed (updates `IsCompleted`).

### MapShowTmdbJob

**Trigger**: Enqueued as continuation from RefreshAvailableShowsJob  
**Concurrency**: Max 1 (sequential)  

**Behavior**:
1. Fetches all shows without TMDB IDs
2. Resolves each via `IShowTmdbMapper.TryResolveShowAsync()` (with name cleaning); unmatched shows are retried as movies via `TryResolveMovieAsync()`
3. Sets `TmdbId`, `IsCompleted`, `TvdbId` on matching shows
4. Bulk saves every 50 shows

## Subtitle Jobs

### FetchSubtitlesJob

**Trigger**: Enqueued when subtitle search finds missing episodes  
**Queue**: `fetch-subtitles`  
**Concurrency**: Max 10 concurrent  
**Retry**: 20 attempts, exponential backoff  
**Timeout**: 10 minutes  

**Job Data**:
```csharp
readonly record struct JobData(long ShowId, int Season, int Episode, Culture? Language, string? FileName);
```

**Behavior**:
1. Acquires async keyed lock per (showId, season) — skips if the lock is already held
2. Enqueues a `CleanupEmptySeasonsJob` continuation for the show
3. Refreshes the season via `ISeasonRefresher.GetRefreshSeasonAsync()` and the episode via `IEpisodeRefresher.GetRefreshEpisodeAsync()` (refresh intervals are checked by the refreshers per `RefreshConfig`)
4. Logs whether a matching subtitle was found; episodes not discovered in the last 180 days are flagged in the log as stale

### CleanupEmptySeasonsJob

**Trigger**: Hangfire continuation after `FetchSubtitlesJob` and `RefreshSingleShowJob`  
**Queue**: `default`  
**Dedup**: `[UniqueJob]`

**Behavior**:
1. Deletes seasons of the show that have **neither episodes nor season packs** (via `ISeasonRepository.DeleteEmptySeasonsForShowAsync()`)
2. Seasons that still hold a season pack are preserved (packs are treated as potential content)

### StoreSubtitleJob

**Trigger**: Enqueued after successful subtitle download  
**Queue**: `store-subtitle`  

**Behavior**:
1. Receives subtitle UniqueId and file blob
2. Acquires async keyed lock per subtitle
3. Stores to compressed storage: `{showId}/{season}/{episode}/{uniqueId}.srt`
4. Updates subtitle record with `StoragePath` and `StoredAt`

### StoreSeasonPackJob

**Trigger**: Enqueued after a season pack is downloaded (ZIP) or when packs are cataloged  
**Queue**: `store-subtitle`  
**Dedup**: `[UniqueJob]`

**Behavior**: Downloads the season pack ZIP from upstream, stores it under the v2 S3 prefix, and updates the `SeasonPackSubtitle` record's `StoragePath`/`StoredAt`.

### Season Pack Storage Migration Jobs

`ResetSeasonPackStorageForV2PrefixMigration` / `EnqueueSeasonPackRedownloadMigration` (+ `...Again`) clear season-pack storage metadata and enqueue forced re-download/re-store jobs (`StoreSeasonPackJob`) so all packs end up under the v2 prefix; `RecatalogStoredSeasonPacksMigration` re-catalogs every stored pack from storage and replaces stale entry rows.

## One-Time Migration Framework

The `AddictedProxy.OneTimeMigration` project provides a framework for running data migrations exactly once at application startup. This is separate from EF Core schema migrations.

### How It Works

1. **Interface**: Migrations implement `IMigration` with a `[MigrationDate]` attribute
2. **Registration**: Migrations are registered in `BootstrapMigration` as `IServiceCollection` entries
3. **Discovery**: `MigrationRunnerHostedService` runs on startup, finding pending migrations
4. **Execution**: Pending migrations are enqueued as Hangfire jobs via `RunnerJob`
5. **Tracking**: `OneTimeMigrationRelease` table tracks state (Running → Success/Fail)

### IMigration Interface

```csharp
[MigrationDate(year, month, day)]
public class MyMigration : IMigration
{
    public async Task ExecuteAsync(CancellationToken token)
    {
        // Migration logic here
    }
}
```

The `MigrationType` property auto-generates a name: `"YYYY-M-D_ClassName"`.

### Existing Migrations

| Migration | Date | Purpose |
|-----------|------|---------|
| `PopulateTvDbIdsMigration` | 2023-02-29 | Fetch TvDB IDs from TMDB external IDs |
| `SetCreatedDateAndUpdatedDateEpisodesMigration` | 2023-09-16 | Backfill `CreatedAt`/`UpdatedAt` on episodes |
| `SetCreatedDateAndUpdatedDateSubtitlesMigration` | 2023-09-16 | Backfill `CreatedAt`/`UpdatedAt` on subtitles |
| `CleanUpInboxUsersMigration` | 2024-08-14 | Clean up Addic7ed account inboxes |
| `RemoveOldCheckCompletedJobMigration` | 2025-01-29 | Remove obsolete Hangfire recurring job |
| `MigrateExternalIdsToNewTableMigration` | 2026-02-17 | Populate `ShowExternalId`/`EpisodeExternalId` from legacy `ExternalId` columns |
| `MigrateSubtitleExternalIdMigration` | 2026-02-17 | Populate `Subtitle.ExternalId` from `DownloadUri` |
| `BackportHdToQualitiesMigration` | 2026-02-19 | Backfill `Qualities` bitmask from legacy `HD` flag (HD → Q720P \| Q1080P) |
| `CleanSuperSubtitlesDataMigration` | 2026-02-21 | Truncate and re-import all SuperSubtitles data |
| `BackfillSeasonPackSeasonFkMigration` | 2026-02-24 | Backfill `SeasonId` FK on `SeasonPackSubtitles` by joining on `TvShowId` + `Season` number |
| `CleanupEmptySeasonsMigration` | 2026-03-07 | Delete all seasons with neither episodes nor season packs |
| `CreateMissingSeasonsForSeasonPacksMigration` | 2026-03-09 | Create missing `Season` entities for `SeasonPackSubtitles` and link them via `SeasonId` FK |
| `EnsureUpdatedAtTriggersMigration` | 2026-03-12 | Ensure every table with `UpdatedAt` has the PostgreSQL update trigger |
| `CatalogExistingSeasonPacksMigration` | 2026-03-12 | Catalog stored season packs that do not yet have parsed entry rows |
| `ResetSeasonPackStorageForV2PrefixMigration` | 2026-03-14 | Clear season pack storage metadata before re-storing ZIPs under the v2 prefix |
| `EnqueueSeasonPackRedownloadMigration` | 2026-03-15 | Enqueue forced re-download and re-store jobs for all season packs |
| `RecatalogStoredSeasonPacksMigration` | 2026-03-19 | Re-catalog every stored season pack from storage and replace stale entry rows |
| `EnqueueSeasonPackRedownloadMigrationAgain` | 2026-03-19 | Re-enqueue forced re-download/re-store for all season packs (v2 prefix) |
| `CleanupEmptySeasonsMigrationAgain` | 2026-08-08 | Re-run of the empty-season cleanup (pollution re-accumulated via SuperSubtitles packs) |
| `CleanupAddictedShowIdZeroSeasonsMigration` | 2026-08-10 | Remove bogus seasons/episodes created when legacy `ExternalId=0` shows were queried on Addic7ed |
| `NormalizeSubtitleSceneSeparatorMigration` | 2026-08-10 | Normalize Addic7ed `Scene` values from `+`-joined release groups to comma-separated |

### Registration

Migrations are auto-registered: `AddictedProxy/Migrations/Bootstrap/BootstrapMigration.cs` declares

```csharp
public partial class BootstrapMigration : IBootstrap, IBootstrapAutoRegister<IMigration>;
```

The `IBootstrapAutoRegister<IMigration>` marker makes the bootstrap framework scan the assembly and register every `IMigration` implementation automatically — no per-class `services.AddScoped<IMigration, ...>()` lines needed when adding a new migration.

### Data Migration vs. Schema Migration

| Aspect | EF Core Migrations | One-Time Migrations |
|--------|-------------------|---------------------|
| **Purpose** | Schema changes (tables, columns, indexes) | Data transformations (backfill, cleanup) |
| **Location** | `AddictedProxy.Database/Migrations/` | `AddictedProxy/Migrations/Services/` |
| **Execution** | `dbContext.Database.MigrateAsync()` at startup | Hangfire background jobs after startup |
| **Tracking** | EF `__EFMigrationsHistory` table | `OneTimeMigrationRelease` table |
| **Idempotent** | Yes (EF tracks applied migrations) | Yes (checks state before running) |
| **Rollback** | Supported by EF | Manual (no built-in rollback) |

## Credential Refresh Jobs

Recurring jobs scheduled by `SchedulerHostedService`:
- **`DownloadCredsRedeemerJob`** (hourly at :11, cron `11 * * * *`): redeems expired download credentials via `ICredentialsService.RedeemDownloadCredentialsAsync()` — resets credentials whose `DownloadExceededDate` cooldown (`Refresh:DownloadExceededTimeout`) has passed
- **`ResetDownloadCredJob.CheckAndResetCredAsync`** (enqueued per credential by `CredentialsService`; `[UniqueJob(TtlFingerprintSeconds = 1h)]`, queue `download-creds-checker`, max 3 concurrent, 10 retry attempts): checks and resets a single credential's download status

## SuperSubtitles Jobs

See [Multi-Provider Plan](multi-provider-plan.md) for full details.

### ImportSuperSubtitlesJob (One-Time Startup Job)

**Trigger**: Enqueued once on startup when `SuperSubtitles:Import:EnableImport=true`  
**Purpose**: Bulk import all shows and subtitles from the SuperSubtitles gRPC API  
**Dedup**: `[UniqueJob]` (not re-enqueued while a previous import is pending)

**Notes**:
- Idempotent: skips when a max subtitle cursor already exists.
- Uses one database transaction per configured show batch (not per streamed item).

**Behavior**:
1. Calls `GetShowListAsync()` via gRPC (streams `Show` objects, consumed asynchronously and collected)
2. Splits collected shows into configurable batches (`SuperSubtitles:Import:BatchSize`, default 60)
3. For each batch: calls `GetShowSubtitlesAsync()` (streams `ShowSubtitlesCollection`, each message containing a complete show with all its subtitles)
4. Processes stream asynchronously: ShowInfo contains show metadata + third-party IDs, Subtitle objects follow
5. Looks up `ShowExternalId(Source=SuperSubtitles)` first for already-imported shows; falls back to TvDB/TMDB matching from third-party IDs
6. Separates season packs (`is_season_pack = true`) and stores them in `SeasonPackSubtitle` table (with `SeasonId` FK resolved via `IngestSeasonPacksAsync`)
7. Uses `season` and `episode` fields from the proto `Subtitle` message directly for episode subtitles
8. Upserts episodes and subtitles via `EpisodeRepository.UpsertEpisodes()`
9. **Waits between batches** with a random delay between `MinDelaySeconds` and `MaxDelaySeconds` (defaults 5–10) to avoid upstream rate limiting
10. Stores the max subtitle ID as a cursor for subsequent incremental updates

### RefreshSuperSubtitlesJob (Recurring)

**Trigger**: Scheduled recurring job every 15 minutes (Hangfire cron: `*/15 * * * *`), only registered when `SuperSubtitles:Import:EnableRefresh=true`  
**Purpose**: Incrementally fetch new subtitles from SuperSubtitles since the last known subtitle ID  
**Dedup**: `[UniqueJob]`

**Behavior**:
1. Loads the stored max SuperSubtitles subtitle ID (skips immediately if the bulk import cursor is not yet set)
2. Calls `CheckForUpdatesAsync()` via gRPC with the stored ID
3. If no updates → exits early
4. Calls `GetRecentSubtitlesAsync(since_id)` (streams `ShowSubtitlesCollection`, each message containing a complete show with its new subtitles)
5. Processes stream asynchronously: ShowInfo sent once per show, followed by new Subtitle objects
6. Looks up `ShowExternalId(Source=SuperSubtitles)` first; falls back to TvDB/TMDB matching from third-party IDs
7. Matches/merges shows and upserts episodes + subtitles (same logic as bulk import)
8. Stores season packs in `SeasonPackSubtitle` table (with `SeasonId` FK resolved via `IngestSeasonPacksAsync`)
9. Updates the stored max subtitle ID

The refresh execution processes the streamed incremental payload in a single database transaction for the run.

# Background Jobs

## Overview

AddictedProxy uses **Hangfire** with PostgreSQL storage for background job processing. Jobs handle show/episode refresh, subtitle storage, TMDB mapping, and one-time data migrations. Each job type runs on a dedicated queue with configurable concurrency.

**Hangfire.Console** is integrated to provide real-time progress tracking and console output within the Hangfire Dashboard for all background jobs.

## Job Infrastructure

### Queues and Concurrency

| Queue | Job | Max Concurrency | Purpose |
|-------|-----|-----------------|---------|
| `default` | Various | Default | General purpose |
| `fetch-subtitles` | FetchSubtitlesJob | 10 | Fetch episodes/subtitles from upstream |
| `refresh-one-show` | RefreshSingleShowJob | 6 | Refresh single show metadata |
| `store-subtitle` | StoreSubtitleJob | Default | Store subtitle files to S3 |

### Job Attributes

Custom Hangfire attributes used:
- **`[UniqueJob(Order, TTL)]`**: Prevents duplicate jobs (deduplication by parameters)
- **`[MaxConcurrency(n)]`**: Limits concurrent execution
- **`[AutoRetry(attempts, backoffType)]`**: Automatic retry with exponential backoff

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
RefreshAvailableShowsJob
    │
    ├─► ShowRefresher.RefreshShowsAsync()     // Fetch all shows from Addic7ed
    │
    └─► ContinueJobWith:
        ├─► MapShowTmdbJob                    // Map shows to TMDB
        │       └─► ContinueJobWith:
        │           ├─► CleanDuplicateTmdbJob  // Clean duplicate TMDB mappings
        │           └─► FetchMissingTvdbIdJob   // Fetch missing TvDB IDs
        └─► (pipeline continues)
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

### MapShowTmdbJob

**Trigger**: Continuation from RefreshAvailableShowsJob  
**Concurrency**: Max 1 (sequential)  

**Behavior**:
1. Fetches all shows without TMDB IDs
2. Searches TMDB API by show name (with name cleaning)
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
record JobData(long ShowId, int Season, int Episode, Culture Language, string? FileName);
```

**Behavior**:
1. Acquires async keyed lock per (showId, season)
2. Checks if season needs refresh (configurable intervals)
3. Refreshes seasons via `ISeasonRefresher`
4. Refreshes episodes/subtitles via `IEpisodeRefresher`
5. Only re-fetches if 180+ days since last refresh

### StoreSubtitleJob

**Trigger**: Enqueued after successful subtitle download  
**Queue**: `store-subtitle`  

**Behavior**:
1. Receives subtitle UniqueId and file blob
2. Acquires async keyed lock per subtitle
3. Stores to compressed storage: `{showId}/{season}/{episode}/{uniqueId}.srt`
4. Updates subtitle record with `StoragePath` and `StoredAt`

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

### Registration

```csharp
// AddictedProxy/Migrations/Bootstrap/BootstrapMigration.cs
public class BootstrapMigration : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<IMigration, PopulateTvDbIdsMigration>();
        services.AddScoped<IMigration, SetCreatedDateAndUpdatedDateEpisodesMigration>();
        // ... etc
    }
}
```

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

### Background credential management:
- Tracks download usage per Addic7ed account
- Resets exceeded credentials after cooldown period
- Monitors account health via `GetDownloadUsageAsync()`

## SuperSubtitles Jobs

See [Multi-Provider Plan](multi-provider-plan.md) for full details.

### ImportSuperSubtitlesJob (One-Time Startup Job)

**Trigger**: Enqueued once on startup when `SuperSubtitles:Import:EnableImport=true`  
**Purpose**: Bulk import all shows and subtitles from the SuperSubtitles gRPC API  
**Concurrency**: Max 1  

**Notes**:
- Idempotent: skips when a max subtitle cursor already exists.
- Uses one database transaction per configured show batch (not per streamed item).

**Behavior**:
1. Calls `GetShowList()` via gRPC (streams `Show` objects, consumed asynchronously and collected into batches)
2. Splits collected shows into configurable batches (e.g. 10 shows per batch)
3. For each batch: calls `GetShowSubtitles()` (streams `ShowSubtitleItem` containing `ShowInfo` + `Subtitle` objects linked by show_id)
4. Processes stream asynchronously: ShowInfo contains show metadata + third-party IDs, Subtitle objects follow
5. Looks up `ShowExternalId(Source=SuperSubtitles)` first for already-imported shows; falls back to TvDB/TMDB matching from third-party IDs
6. Separates season packs (`is_season_pack = true`) and stores them in `SeasonPackSubtitle` table
7. Uses `season` and `episode` fields from the proto `Subtitle` message directly for episode subtitles
8. Upserts episodes and subtitles via `EpisodeRepository.UpsertEpisodes()`
9. **Waits between batches** (configurable delay, e.g. 3 seconds) to avoid upstream rate limiting
10. Stores the max subtitle ID as a cursor for subsequent incremental updates

### RefreshSuperSubtitlesJob (Recurring)

**Trigger**: Scheduled recurring job every 15 minutes (Hangfire cron: `*/15 * * * *`)  
**Purpose**: Incrementally fetch new subtitles from SuperSubtitles since the last known subtitle ID  
**Concurrency**: Max 1  

**Behavior**:
1. Loads the stored max SuperSubtitles subtitle ID
2. Calls `CheckForUpdates()` via gRPC with the stored ID
3. If no updates → exits early
4. Calls `GetRecentSubtitles(since_id)` (streams `ShowSubtitleItem` containing `ShowInfo` + `Subtitle` objects for shows with new subtitles)
5. Processes stream asynchronously: ShowInfo sent once per show, followed by new Subtitle objects
6. Looks up `ShowExternalId(Source=SuperSubtitles)` first; falls back to TvDB/TMDB matching from third-party IDs
7. Matches/merges shows and upserts episodes + subtitles (same logic as bulk import)
8. Stores season packs in `SeasonPackSubtitle` table
9. Updates the stored max subtitle ID

The refresh execution processes the streamed incremental payload in a single database transaction for the run.

# Architecture Overview

## Introduction

AddictedProxy (branded as **Gestdown**) is a .NET 10 ASP.NET Core application that provides a proxy API for searching and downloading subtitles. It includes a Nuxt 4 (Vue.js) frontend, PostgreSQL database, background job scheduling with Hangfire, and comprehensive observability via OpenTelemetry and Sentry.

**License:** GPL-3.0

## Tech Stack

| Layer           | Technology                                      |
|-----------------|--------------------------------------------------|
| Runtime         | .NET 10.0, ASP.NET Core                         |
| Database        | PostgreSQL 18 via EF Core 10 + Npgsql            |
| Caching         | PostgreSQL (primary) + Redis (optional) + In-Memory |
| Jobs            | Hangfire with PostgreSQL storage                 |
| Observability   | OpenTelemetry, Sentry, Prometheus                |
| Compression     | ZstdSharp.Port                                   |
| Frontend        | Nuxt 4, Vue.js 3, Vuetify 3, pnpm               |
| Testing         | NUnit 4, NSubstitute, FluentAssertions           |
| CI/CD           | GitHub Actions, semantic-release                 |
| Container       | Docker (Alpine-based), Docker Compose            |

## Repository Structure

```
AddictedProxy/                    # Main ASP.NET Core web application (entry point)
├── Controllers/Rest/             # REST API controllers
├── Controllers/Hub/              # SignalR hubs (real-time updates)
├── Services/                     # Business logic services
│   ├── Provider/                 # Core provider services (shows, episodes, seasons, subtitles)
│   ├── Search/                   # Subtitle search engine
│   ├── Credentials/              # Upstream credential management
│   ├── Details/                  # Media details (TMDB integration)
│   ├── Job/                      # Background job infrastructure
│   └── Sitemap/                  # Sitemap generation
├── Migrations/                   # One-time data migrations (not EF schema migrations)
├── Model/                        # DTOs and response models
└── Program.cs                    # Application entry point and DI configuration

AddictedProxy.Database/           # EF Core DbContext, entities, repositories
├── Context/                      # EntityContext (DbContext)
├── Model/                        # Entity definitions (TvShow, Episode, Season, Subtitle, etc.)
├── Repositories/                 # Repository pattern implementations
└── Migrations/                   # EF Core schema migrations (auto-applied at startup)

AddictedProxy.Upstream/           # Upstream Addic7ed communication
├── Service/                      # Client, downloader, parser for Addic7ed
├── Model/                        # Upstream-specific models (SubtitleRow, DownloadUsage)
└── Boostrap/                     # DI registration for upstream services

SuperSubtitleClient/              # SuperSubtitles gRPC client wrapper
                                  # Provides streaming API access used by provider jobs

AddictedProxy.Caching/            # Caching abstractions and implementations
AddictedProxy.Culture/            # Culture/language parsing utilities
AddictedProxy.Image/              # Image processing with ImageSharp
AddictedProxy.OneTimeMigration/   # One-time data migration framework
AddictedProxy.Stats/              # Statistics tracking (show popularity)
AddictedProxy.Storage/            # Storage abstraction (AWS S3)
AddictedProxy.Storage.Caching/    # Cached storage layer (combines cache + S3)
AddictedProxy.Tools.Database/     # Database tooling helpers (transactions)
AntiCaptcha/                      # CAPTCHA solving integration (DORMANT — still in the repo, but no bootstrap references it; preserved for potential reuse)
Compressor/                       # Zstandard compression utilities
InversionOfControl/               # Custom DI bootstrap framework
Locking/                          # Async keyed locking utilities
Performance/                      # OpenTelemetry tracing/metrics
ProxyProvider/                    # HTTP proxy provider abstraction
ProxyScrape/                      # ProxyScrape v4 API integration (residential proxy quota metrics)
TvMovieDatabaseClient/            # TMDB API client
addicted.nuxt/                    # Nuxt 4 frontend (Vue.js + Vuetify)
```

## Bootstrap Pattern (Dependency Injection)

The project uses a custom DI bootstrap system in the `InversionOfControl` project. Each module registers its services by implementing `IBootstrap` (for service registration) and/or `IBootstrapApp` (for middleware/app configuration).

```csharp
// Interface
public interface IBootstrap
{
    void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging);
}
```

### Discovery via Assembly Scanning

In `Program.cs`, all module assemblies are passed to `AddBootstrap`/`UseBootstrap`:

```csharp
var currentAssemblies = new[]
{
    typeof(BootstrapController).Assembly,
    typeof(BootstrapDatabase).Assembly,
    typeof(BootstrapCompressor).Assembly,
    typeof(BootstrapAddictedServices).Assembly,
    typeof(BootstrapPerformanceSentry).Assembly,
    typeof(BootstrapStatsPopularityShow).Assembly,
    typeof(BootstrapTMDB).Assembly,
    typeof(BootstrapRedisCaching).Assembly,
    typeof(BootstrapCulture).Assembly,
    typeof(BootstrapStorageCaching).Assembly,
    typeof(BootstrapMigration).Assembly,
    typeof(BootstrapImage).Assembly,
    typeof(BootstrapStoreCompression).Assembly,
    typeof(BootstrapProxyScrape).Assembly
};

builder.AddBootstrap(currentAssemblies);
// ... later ...
app.UseBootstrap(currentAssemblies);
```

When adding a new module, create a class implementing `IBootstrap` in the module project. If the module's assembly isn't already passed to `AddBootstrap`/`UseBootstrap`, add it in `Program.cs`.

Conditional bootstrapping is supported via `IBootstrapConditional` (checked at registration time) and environment variable parsing via `IBootstrapEnvironmentVariable<T, TParser>`.

## Data Flow (High Level)

```
                    ┌──────────────────┐
                    │   Nuxt Frontend  │
                    └────────┬─────────┘
                             │ HTTP
                    ┌────────▼─────────┐
                    │  ASP.NET Core    │
                    │  REST Controllers│
                    └────────┬─────────┘
                             │
              ┌──────────────┼──────────────┐
              │              │              │
     ┌────────▼───────┐ ┌───▼────┐ ┌───────▼──────┐
     │ Search Service │ │ Subtitle│ │ Details      │
     │                │ │Provider │ │ Provider     │
     └────────┬───────┘ └───┬────┘ └───────┬──────┘
              │             │              │
     ┌────────▼───────┐ ┌───▼────┐ ┌───────▼──────┐
     │ Show/Season/   │ │Download│ │  TMDB Client  │
     │ Episode        │ │ +Store │ │               │
     │ Refreshers     │ └───┬────┘ └──────────────┘
     └────────┬───────┘     │
              │             │
     ┌────────▼─────────────▼──────┐
     │     Repository Layer        │
     │  (ITvShowRepository, etc.)  │
     └────────────┬────────────────┘
                  │
     ┌────────────▼────────────────┐
     │   PostgreSQL (EF Core)      │
     └─────────────────────────────┘
              │
     ┌────────▼───────────┐
     │  Upstream Module   │
     │  (Addic7ed Client) │──────► addic7ed.com
     └───────────────────┘
              │
     ┌────────▼─────────────────┐
      │  SuperSubtitles Module   │
      │  (gRPC Client + Jobs)    │──────► SuperSubtitles gRPC API ──► feliratok.eu
     └──────────────────────────┘
```

## SuperSubtitles Ingestion & Validation

SuperSubtitles is the secondary subtitle provider (upstream feliratok.eu). Its ingestion
runs through two Hangfire jobs — `ImportSuperSubtitlesJob` (one-time bulk import on
startup) and `RefreshSuperSubtitlesJob` (incremental, recurring every 15 minutes).

**Season validation on ingestion.** Because feliratok.eu can return polluted seasons for a
show, each streamed collection is passed through
`SuperSubtitlesStreamFilter.DropInvalidSeasons(TvShow, subtitles, logger)` before it is
persisted. This static filter drops proto subtitles whose season exceeds the show's known
season count derived from TMDB:

- Season `0` (specials) is always kept.
- If the show's season count is unknown, every season is kept.
- The max-subtitle-id cursor advances over the **raw** stream, so dropped entries are not
  re-requested on the next run.

The show's season count is stored as `TvShow.NumberOfSeasons` (`int?`, added by the EF Core
migration `20260808224126_AddTvShowNumberOfSeasons`), populated by `ShowTmdbMapper` from
TMDB show details and reset to `null` when a show is removed from TMDB.

**Pruning bogus seasons.** Both SuperSubtitles jobs register a `CleanupEmptySeasonsJob`
Hangfire continuation per processed show. Degrading empty seasons that escape ingestion
(e.g. historically created bogus seasons 5/8/9 on a 1-season show) are removed by
`PruneInvalidSeasonsJob` (`[UniqueJob]`, per-show async keyed lock), which deletes any
seasons beyond the TMDB count via `ISeasonRepository.DeleteSeasonsBeyondAsync`. That
repository method is FK-safe: it issues an ordered `ExecuteDelete` chain (subtitles →
episode external IDs → season pack entries → season packs → episodes → seasons, with
`IgnoreQueryFilters` on the pack tables for soft-deleted rows).

**Progressive retroactive healing.** Because ingestion validation only protects new writes,
two jobs heal pre-existing data:

- `CheckCompletedTmdbJob` (recurring) backfills `NumberOfSeasons` from TMDB for completed
  shows and enqueues `PruneInvalidSeasonsJob` for a show when its count changes.
- `MapShowTmdbJob` enqueues `PruneInvalidSeasonsJob` for newly mapped shows.

## Application Startup

1. **Configuration**: Environment variables with `A7D_` prefix, `appsettings.json`
2. **DI Bootstrap**: All module assemblies scanned for `IBootstrap` implementations
3. **Database Migration**: `dbContext.Database.MigrateAsync()` applies pending EF Core migrations
4. **One-Time Migrations**: `MigrationRunnerHostedService` enqueues pending data migrations via Hangfire
5. **Hangfire**: Background job processing starts
6. **HTTP Pipeline**: Controllers, SignalR hubs, Swagger, response caching

## Configuration

- **Environment variables**: Use `A7D_` prefix convention
- **Settings files**: `appsettings.json` and `appsettings.Development.json`
- **Key config sections**: Connection strings, PostgreSQL caching, rate limiting, proxy scraping, Sentry, Performance (OpenTelemetry)
- **NuGet packages**: Centrally managed in `Directory.Packages.props` at solution root

## Observability

- **Tracing**: OpenTelemetry spans via `IPerformanceTracker` throughout services and jobs
- **Metrics**: Prometheus metrics (e.g., download counters via `DownloadCounterWrapper`)
- **Error Tracking**: Sentry integration with environment-specific configuration. Upstream parse/resilience failures are classified so transient faults don't flood Sentry: the Addic7ed parser throws `NothingToParseException` (via a null-safe guard for a missing episode table) instead of leaking an `ArgumentNullException`, and `FetchSubtitlesJob` treats `NothingToParseException`/`BrokenCircuitException` as transient (`Status.Unavailable` + warning, rethrown) rather than `LogCritical`.
- **Logging**: Structured logging via `ILogger<T>`. Polly's `OnTimeout` error-level telemetry is silenced via `"Polly": "Critical"` in `appsettings.json:Logging:LogLevel`.

## Key Design Patterns

| Pattern               | Usage                                                        |
|-----------------------|--------------------------------------------------------------|
| Repository Pattern    | Database access abstracted via interfaces                   |
| Service Layer         | Business logic isolated from controllers                    |
| Bootstrap Pattern     | Custom DI framework for modular service registration        |
| Background Jobs       | Hangfire for async/scheduled work (refresh, store, migrate)  |
| Async Keyed Locking   | Prevents concurrent operations on same resource             |
| HTTP Resilience       | Polly v8 resilience pipelines via `Microsoft.Extensions.Http.Resilience`; shared retry + circuit breaker (5xx, 401/402/403) + timeout for all upstream HTTP/gRPC clients. Addic7ed: 8 retry attempts (exponential 10–60 s), circuit breaker 5 min break (0.5 failure ratio / 20 minimum throughput), 3 min per-attempt timeout plus a 5 min overall `HttpClient.Timeout` via `AddSharedResilienceHandler`/explicit config |
| Real-time Updates     | SignalR hubs for progress notifications                     |
| Caching               | Multi-layer: In-Memory → Redis → PostgreSQL                |
| Compression           | Zstandard compression for stored subtitle files             |

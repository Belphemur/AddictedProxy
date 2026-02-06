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

AddictedProxy.Caching/            # Caching abstractions and implementations
AddictedProxy.Culture/            # Culture/language parsing utilities
AddictedProxy.Image/              # Image processing with ImageSharp
AddictedProxy.OneTimeMigration/   # One-time data migration framework
AddictedProxy.Stats/              # Statistics tracking (show popularity)
AddictedProxy.Storage/            # Storage abstraction (AWS S3)
AddictedProxy.Storage.Caching/    # Cached storage layer (combines cache + S3)
AddictedProxy.Tools.Database/     # Database tooling helpers (transactions)
AntiCaptcha/                      # CAPTCHA solving integration
Compressor/                       # Zstandard compression utilities
InversionOfControl/               # Custom DI bootstrap framework
Locking/                          # Async keyed locking utilities
Performance/                      # OpenTelemetry tracing/metrics
ProxyProvider/                    # HTTP proxy provider abstraction
ProxyScrape/                      # Proxy scraping implementation
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
```

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
- **Error Tracking**: Sentry integration with environment-specific configuration
- **Logging**: Structured logging via `ILogger<T>`

## Key Design Patterns

| Pattern               | Usage                                                        |
|-----------------------|--------------------------------------------------------------|
| Repository Pattern    | Database access abstracted via interfaces                   |
| Service Layer         | Business logic isolated from controllers                    |
| Bootstrap Pattern     | Custom DI framework for modular service registration        |
| Background Jobs       | Hangfire for async/scheduled work (refresh, store, migrate)  |
| Async Keyed Locking   | Prevents concurrent operations on same resource             |
| HTTP Resilience       | Polly retry policies, circuit breakers, timeouts            |
| Real-time Updates     | SignalR hubs for progress notifications                     |
| Caching               | Multi-layer: In-Memory → Redis → PostgreSQL                |
| Compression           | Zstandard compression for stored subtitle files             |

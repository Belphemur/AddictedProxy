# Provider System

## Overview

The provider system is responsible for fetching show metadata and subtitle data from upstream subtitle sources. Currently, AddictedProxy has a single provider: **Addic7ed**. The system is designed around service interfaces that abstract the operations of refreshing shows, seasons, episodes, and downloading subtitles.

## Current Architecture (Single Provider: Addic7ed)

### Service Layer

All provider services are registered via `BootstrapProvider` in `AddictedProxy/Services/Provider/Bootstrap/`:

```csharp
public class BootstrapProvider : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<IShowRefresher, ShowRefresher>();
        services.AddScoped<ISubtitleProvider, SubtitleProvider>();
        services.AddScoped<ISeasonRefresher, SeasonRefresher>();
        services.AddScoped<IEpisodeRefresher, EpisodeRefresher>();
        services.Configure<RefreshConfig>(configuration.GetSection("Refresh"));
        services.AddSingleton<IRefreshHubManager, RefreshHubManager>();
        services.AddScoped<SubtitleCounterUpdater>();
        services.AddScoped<IDetailsProvider, DetailsProvider>();
    }
}
```

### Core Interfaces

#### IShowRefresher

Responsible for fetching and managing the show catalog.

```csharp
public interface IShowRefresher
{
    Task RefreshShowsAsync(CancellationToken token);
    IAsyncEnumerable<TvShow> FindShowsAsync(string search, CancellationToken token);
    Task<TvShow?> GetShowByGuidAsync(Guid id, CancellationToken cancellationToken);
    IAsyncEnumerable<TvShow> GetShowByTvDbIdAsync(int id, CancellationToken cancellationToken);
    Task RefreshShowAsync(long tvShow, CancellationToken token);
}
```

**Current Implementation (`ShowRefresher`):**
- `RefreshShowsAsync()`: Calls `IAddic7edClient.GetTvShowsAsync()` to fetch all shows, then upserts them into the database
- `RefreshShowAsync()`: Refreshes seasons and episodes for a specific show using `ISeasonRefresher` and `IEpisodeRefresher`
- `FindShowsAsync()`: Delegates to `ITvShowRepository.FindAsync()` for full-text search
- Sends real-time progress via `IRefreshHubManager` (SignalR)

#### ISeasonRefresher

Responsible for fetching season information for shows.

```csharp
public interface ISeasonRefresher
{
    Task<Season?> GetRefreshSeasonAsync(TvShow show, int seasonNumber, CancellationToken token);
    Task RefreshSeasonsAsync(TvShow show, CancellationToken token = default);
    bool IsShowNeedsRefresh(TvShow show);
}
```

**Current Implementation (`SeasonRefresher`):**
- Calls `IAddic7edClient.GetSeasonsAsync()` to fetch seasons from Addic7ed
- Uses async keyed locking to prevent concurrent refreshes per show
- Respects `RefreshConfig` for refresh intervals (completed shows are refreshed less frequently)

#### IEpisodeRefresher

Responsible for fetching episode and subtitle data for specific seasons.

```csharp
public interface IEpisodeRefresher
{
    Task<Episode?> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, CancellationToken token);
    Task RefreshEpisodesAsync(TvShow show, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token);
    bool IsSeasonNeedRefresh(TvShow show, Season season);
}
```

**Current Implementation (`EpisodeRefresher`):**
- Calls `IAddic7edClient.GetEpisodesAsync()` to fetch episodes with their subtitles
- Processes seasons in parallel (chunks of 2)
- Upserts episodes and subtitles via `IEpisodeRepository.UpsertEpisodes()`
- Uses async keyed locking per (showId, seasonId) pair

#### ISubtitleProvider

Responsible for downloading subtitle files.

```csharp
public interface ISubtitleProvider
{
    Task<Stream> GetSubtitleFileAsync(Subtitle subtitle, CancellationToken token);
    Task<Subtitle?> GetSubtitleFullAsync(Guid subtitleId, CancellationToken token);
}
```

**Current Implementation (`SubtitleProvider`):**
- Checks cached storage first (`ICachedStorageProvider`)
- Falls back to downloading via `IAddic7edDownloader.DownloadSubtitle()`
- Rotates credentials via `ICredentialsService` for rate limit distribution
- Retries up to 3 times on `DownloadLimitExceededException`
- Stores completed subtitles in background via `StoreSubtitleJob`

#### IDetailsProvider

Responsible for fetching media details (posters, descriptions, etc.) from TMDB.

```csharp
public interface IDetailsProvider
{
    Task<(ShowDetails details, ExternalIds? externalIds)?> GetShowInfoAsync(TvShow show, CancellationToken token);
    Task<(MovieDetails details, ExternalIds? externalIds)?> GetMovieInfoAsync(TvShow show, CancellationToken token);
}
```

### Upstream Module (Addic7ed)

The `AddictedProxy.Upstream` project contains the Addic7ed-specific client implementation:

```
AddictedProxy.Upstream/
├── Service/
│   ├── IAddic7edClient.cs        # Interface for Addic7ed API
│   ├── Addic7edClient.cs         # HTTP client implementation
│   ├── IAddic7edDownloader.cs    # Interface for subtitle file download
│   ├── Addic7edDownloader.cs     # Download implementation with retry
│   ├── Parser.cs                 # HTML parser (AngleSharp) for scraping
│   ├── HttpUtils.cs              # Request preparation with credentials
│   └── Performance/              # Metrics (download counters)
├── Model/
│   ├── SubtitleRow.cs            # Parsed subtitle data from HTML
│   └── DownloadUsage.cs          # Account usage tracking
└── Boostrap/
    └── BootstrapAddictedServices.cs  # DI registration
```

#### IAddic7edClient

```csharp
public interface IAddic7edClient
{
    IAsyncEnumerable<TvShow> GetTvShowsAsync(AddictedUserCredentials creds, CancellationToken token);
    Task<IEnumerable<Season>> GetSeasonsAsync(AddictedUserCredentials credentials, TvShow show, CancellationToken token);
    Task<IEnumerable<Episode>> GetEpisodesAsync(AddictedUserCredentials credentials, TvShow show, int season, CancellationToken token);
    Task<DownloadUsage> GetDownloadUsageAsync(AddictedUserCredentials credentials, CancellationToken token);
    Task<bool> CleanupInbox(AddictedUserCredentials creds, CancellationToken token);
}
```

#### IAddic7edDownloader

```csharp
public interface IAddic7edDownloader
{
    Task<Stream> DownloadSubtitle(AddictedUserCredentials? credentials, Subtitle subtitle, CancellationToken token);
}
```

### Data Flow: Refresh Shows

```
RefreshAvailableShowsJob (Hangfire scheduled)
    │
    ▼
ShowRefresher.RefreshShowsAsync()
    │
    ├─► ICredentialsService.GetLeastUsedCredsQueryingAsync()  // Get credentials
    │
    ├─► IAddic7edClient.GetTvShowsAsync()                    // Fetch shows from Addic7ed
    │       │
    │       └─► Parser.GetShowsAsync()                        // Parse HTML response
    │
    └─► ITvShowRepository.UpsertRefreshedShowsAsync()         // Save to database
```

### Data Flow: Search & Download Subtitle

```
SubtitlesController (GET /subtitles/get/{showId}/{season}/{episode}/{language})
    │
    ▼
SearchSubtitlesService.FindSubtitlesAsync()
    │
    ├─► IEpisodeRepository.GetEpisodeUntrackedAsync()    // Check DB for episode
    │
    ├─► If missing: Enqueue FetchSubtitlesJob            // Background refresh
    │       │
    │       └─► EpisodeRefresher → IAddic7edClient.GetEpisodesAsync()
    │
    └─► Return matching subtitles by language
    
SubtitlesController (GET /subtitles/download/{subtitleId})
    │
    ▼
SubtitleProvider.GetSubtitleFileAsync()
    │
    ├─► Check ICachedStorageProvider                     // Try cached file first
    │
    └─► If not cached:
        ├─► ICredentialsService.GetLeastUsedCredsDownloadAsync()
        ├─► IAddic7edDownloader.DownloadSubtitle()       // Download from Addic7ed
        ├─► Enqueue StoreSubtitleJob                     // Cache in background
        └─► Return stream
```

### Credential Management

Addic7ed enforces rate limits per account. The system manages multiple `AddictedUserCredentials` entries:

- **`ICredentialsService`**: Selects the least-used credential for queries and downloads
- **Usage tracking**: Each credential tracks query count and download count
- **Rate limit handling**: When a download limit is exceeded, the credential is tagged (`DownloadExceededDate`) and the system tries another credential
- **Rotation**: Credentials are selected by lowest usage, distributing load across accounts

### Refresh Configuration

```json
{
    "Refresh": {
        "SeasonRefresh": "04:00:00",           // How often to refresh season list
        "EpisodeRefresh": {
            "DefaultRefresh": "12:00:00",       // Default episode refresh interval
            "LastSeasonRefresh": "01:00:00",    // Latest season refreshed more often
            "CompletedShowRefresh": "30.00:00:00" // Completed shows refreshed rarely
        }
    }
}
```

### Provider-Aware Fields

The database already has `Source` fields on key entities:

- **`TvShow.Source`**: `DataSource` enum (default: `Addic7ed`) — where the show was first discovered
- **`Subtitle.Source`**: `DataSource` enum (default: `Addic7ed`) — which provider contributed this subtitle

The `DataSource` enum currently only has `Addic7ed` as a value.

## Planned: SuperSubtitles Provider

See [Multi-Provider Plan](multi-provider-plan.md) for full architecture details.

SuperSubtitles is a Go-based gRPC service that scrapes feliratok.eu (a Hungarian subtitle site). It will be integrated as a second provider with these key characteristics:

- **gRPC client** instead of HTTP/HTML scraping — communicates via Protocol Buffers with the [`supersubtitles.proto`](https://github.com/Belphemur/SuperSubtitles/blob/main/api/proto/v1/supersubtitles.proto) API
- **No credentials needed** — unlike Addic7ed, no authentication or rate-limited credential rotation
- **Season/episode parsing required** — SuperSubtitles does not provide reliable season/episode data; a `SeasonEpisodeParser` will extract this from subtitle `filename`/`name`/`release` fields
- **Async keyed locking** — show match-or-create operations are protected by `AsyncKeyedLocker<long>` keyed on the SuperSubtitles show ID to prevent duplicate `TvShow` creation
- **Two-phase ingestion**: one-time bulk import (with batch delays for rate limiting) + recurring 15-minute incremental updates
- **Subtitle downloads** via gRPC `DownloadSubtitle` method (supports season pack episode extraction)
- **Season packs ignored** during ingestion — only individual episode subtitles are imported

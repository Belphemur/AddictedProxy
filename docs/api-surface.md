# API Surface

## Overview

AddictedProxy exposes a REST API for searching shows, finding subtitles, and downloading subtitle files. It also provides a SignalR hub for real-time progress notifications during show refresh operations.

Base URL: `https://api.gestdown.info`

## REST Endpoints

### Shows Controller (`/shows`)

| Method | Route | Description | Response Cache |
|--------|-------|-------------|----------------|
| `GET` | `/shows/search/{search}` | Search shows by name (min 3 chars) | 1 day |
| `GET` | `/shows/external/tvdb/{tvdbId}` | Find show by TheTVDB ID | 1 day |
| `POST` | `/shows/{showId:guid}/refresh` | Enqueue background refresh for a show | None |
| `GET` | `/shows/{showId:guid}/{seasonNumber:int}/{language}` | Get all subtitles for a season in a language | 2 hours |

#### Search Shows

```
GET /shows/search/{search}
```

- **Parameters**: `search` (string, min 3 chars) — show name to search for
- **Returns**: `ShowSearchResponse` with array of `ShowDto`
- **Cache**: 1 day

#### Get Show by TvDB ID

```
GET /shows/external/tvdb/{tvdbId}
```

- **Parameters**: `tvdbId` (int) — TheTVDB identifier
- **Returns**: `ShowSearchResponse` with matching shows
- **Cache**: 1 day

#### Refresh Show

```
POST /shows/{showId:guid}/refresh
```

- **Parameters**: `showId` (Guid) — show's UniqueId
- **Returns**: 200 OK (refresh queued) or 404 Not Found
- **Side Effect**: Enqueues `RefreshSingleShowJob` via Hangfire

#### Get Season Subtitles

```
GET /shows/{showId:guid}/{seasonNumber:int}/{language}
```

- **Parameters**: `showId` (Guid), `seasonNumber` (int), `language` (string, ISO code)
- **Returns**: `TvShowSubtitleResponse` with episodes and subtitles for that season/language
- **Cache**: 2 hours

### Subtitles Controller (`/subtitles`)

| Method | Route | Description | Response Cache |
|--------|-------|-------------|----------------|
| `GET` | `/subtitles/download/{subtitleId:guid}` | Download a subtitle file | 8 days |
| `POST` | `/subtitles/search` | Search subtitles (deprecated) | 2 hours |
| `GET` | `/subtitles/find/{lang}/{show}/{season}/{episode}` | Find subtitles by name (deprecated) | 2 hours |
| `GET` | `/subtitles/get/{showId:guid}/{season}/{episode}/{language}` | **Preferred**: Get subtitles by show ID | 2 hours |

#### Download Subtitle

```
GET /subtitles/download/{subtitleId:guid}
```

- **Parameters**: `subtitleId` (Guid) — subtitle's UniqueId
- **Returns**: SRT file stream with appropriate headers
- **Headers**: Content-Type `text/srt`, ETag support, file download name
- **Error Codes**: 
  - 404: Subtitle not found or deleted
  - 429: Download rate limit exceeded
- **Cache**: 8 days

#### Get Subtitles (Preferred Endpoint)

```
GET /subtitles/get/{showUniqueId:guid}/{season:int}/{episode:int}/{language}
```

- **Parameters**: 
  - `showUniqueId` (Guid) — from Shows::Search
  - `season` (int, min 0)
  - `episode` (int, min 0)
  - `language` (string, 2+ chars, ISO code)
- **Returns**: `SubtitleSearchResponse` with matching subtitles
- **Error Codes**:
  - 404: Show not found
  - 423: Show is being refreshed, try again later
  - 429: Rate limit

### Media Controller (`/media`)

| Method | Route | Description | Response Cache |
|--------|-------|-------------|----------------|
| `GET` | `/media/trending/{max:range(1,50)}` | Get trending TV shows/movies | 1 day |
| `GET` | `/media/{showId:guid}/details` | Get media details (poster, overview, etc.) | 1 day |
| `GET` | `/media/{showId:guid}/episodes/{language}` | Get last season episodes with subtitles | 2 hours |

### Stats Controller (`/stats`)

Provides statistics endpoints for tracking subtitle and show metrics.

### Sitemap Controller

Generates XML sitemaps for SEO purposes.

## Data Transfer Objects (DTOs)

### ShowDto

```csharp
record ShowDto(
    Guid Id,              // UniqueId
    string Name,
    int NbSeasons,        // Total season count
    int[] Seasons,        // Available season numbers
    int? TvDbId,
    int? TmdbId,
    string Slug           // URL-friendly name
);
```

### SubtitleDto

```csharp
record SubtitleDto(
    int Version,
    bool Completed,
    bool HearingImpaired,
    bool Corrected,
    bool HD,
    string DownloadUri,    // Relative download URL
    string Language,
    long DownloadCount,
    string? Scene          // Release group name
);
```

### EpisodeDto

```csharp
record EpisodeDto(
    int Season,
    int Number,
    string Title,
    string? Show,          // Show name (nullable for inline use)
    DateTime? Discovered
);
```

### EpisodeWithSubtitlesDto

```csharp
record EpisodeWithSubtitlesDto(
    EpisodeDto Episode,
    SubtitleDto[] Subtitles
);
```

### SubtitleSearchResponse

```csharp
record SubtitleSearchResponse(
    IEnumerable<SubtitleDto> MatchingSubtitles,
    EpisodeDto Episode
);
```

### MediaDetailsDto

```csharp
record MediaDetailsDto(
    ShowDto Show,
    DetailsDto Details     // Poster, backdrop, overview, genres, vote, year
);
```

### ErrorResponse

```csharp
record ErrorResponse(string Error);
```

## SignalR Hub

### RefreshHub (`/hub/refresh`)

Provides real-time progress updates when a show's seasons/episodes are being refreshed.

**Methods (Server → Client):**
- `SendProgressAsync(show, progressPercent)` — Progress update (0-100%)
- `SendRefreshDone(show)` — Refresh completed

**Usage**: The Nuxt frontend subscribes to this hub when a user triggers a show refresh, displaying a progress bar.

## Authentication

The API is **public** — no authentication is required for any endpoint. Rate limiting is applied at the proxy/infrastructure level.

## Response Caching Strategy

| Content Type | Cache Duration | Rationale |
|---|---|---|
| Show search results | 1 day | Show catalog changes infrequently |
| Media details/trending | 1 day | TMDB data is stable |
| Subtitle search results | 2 hours | New subtitles appear periodically |
| Season subtitle listing | 2 hours | Same as subtitle search |
| Subtitle file download | 8 days | Subtitle content is immutable once completed |
| Not found responses | 12 hours | Prevents hammering for non-existent content |

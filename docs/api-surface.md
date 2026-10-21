# API Surface

## Overview

AddictedProxy exposes a REST API for searching shows, finding subtitles, and downloading subtitle files.

Base URL: `https://api.gestdown.info`

## ID Encoding

All externally visible API `Guid` identifiers (show ids, subtitle ids, season
pack ids, entry ids) are encoded as **fixed-width base62** strings — 22
characters over the alphabet `0-9A-Za-z` — before leaving the API edge, and
decoded strictly on the way in. See
[2026-10-20-api-id-base62.md](2026-10-20-api-id-base62.md) for the design.

- **Round-trippable**: the server can rebuild the original `Guid` from any id
  it emitted (`Encode(Decode(x)) == x`).
- **Canonical & fail-closed**: only the exact canonical byte form decodes.
  Legacy dashed GUID strings are **rejected** at every surface
  (routes → 404, JSON/download parsing → 400).
- **Alphabet**: `0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`
  (case-sensitive; consumers must preserve case).
- Composite identifiers keep their `sp_` prefix and `_` separators because the
  alphabet excludes `_` and `-`; the encoded Guid half is base62:

| Identifier | Format |
| --- | --- |
| Show / pack / entry id | `{base62}` (22 chars) |
| Subtitle regular | `{base62}` |
| Season pack ZIP | `sp_{base62}` |
| Episode from pack (uncataloged) | `sp_{base62}_ep_{episodeNumber}` |
| Entry from pack (cataloged) | `sp_{base62}_entry_{base62}` |

## REST Endpoints

### Shows Controller (`/shows`)

| Method | Route                                                             | Description                                  | Response Cache |
| ------ | ----------------------------------------------------------------- | -------------------------------------------- | -------------- |
| `GET`  | `/shows/search/{search}`                                          | Search shows by name (min 3 chars)           | 2 hours        |
| `GET`  | `/shows/external/tvdb/{tvdbId}`                                   | Find show by TheTVDB ID                      | 2 hours        |
| `POST` | `/shows/{showId:apiId}/refresh`                                    | Trigger a background refresh of a show       | None           |
| `GET`  | `/shows/{showId:apiId}/{seasonNumber:int}/{language}`              | Get all subtitles for a season in a language | 2 hours        |
| `GET`  | `/shows/{showId:apiId}/{seasonNumber:int}/{language}/season-packs` | Get season pack subtitles for a season       | 2 hours        |

#### Search Shows

```
GET /shows/search/{search}
```

- **Parameters**: `search` (string, min 3 chars) — show name to search for
- **Returns**: `ShowSearchResponse` with array of `ShowDto`
- **Cache**: 2 hours

#### Get Show by TvDB ID

```
GET /shows/external/tvdb/{tvdbId}
```

- **Parameters**: `tvdbId` (int) — TheTVDB identifier
- **Returns**: `ShowSearchResponse` with matching shows
- **Cache**: 2 hours

#### Get Season Subtitles

```
GET /shows/{showId:apiId}/{seasonNumber:int}/{language}
```

- **Parameters**: `showId` (base62-encoded Guid), `seasonNumber` (int), `language` (string, ISO code)
- **Returns**: `TvShowSubtitleResponse` with episodes, subtitles, and season packs for that season/language
- **Cache**: 2 hours

#### Get Season Packs

```
GET /shows/{showId:apiId}/{seasonNumber:int}/{language}/season-packs
```

- **Parameters**: `showId` (base62-encoded Guid), `seasonNumber` (int), `language` (string, ISO code)
- **Returns**: `SeasonPackResponse` with season pack subtitles filtered by language
- **Cache**: 2 hours

### Subtitles Controller (`/subtitles`)

| Method | Route                                                        | Description                             | Response Cache |
| ------ | ------------------------------------------------------------ | --------------------------------------- | -------------- |
| `GET`  | `/subtitles/download/{subtitleId}`                           | Download a subtitle file or season pack | 8 days         |
| `GET`  | `/subtitles/get/{showId:apiId}/{season}/{episode}/{language}` | **Preferred**: Get subtitles by show ID | 2 hours        |

> The legacy name-based endpoints (`POST /subtitles/search`, `GET /subtitles/find/{lang}/{show}/{season}/{episode}`) have been removed — use `/shows/search/{search}` + `/subtitles/get/...` instead.

#### Download Subtitle

```
GET /subtitles/download/{subtitleId}
```

- **Parameters**: `subtitleId` (string) — subtitle's UniqueId or season pack ID
  - Regular subtitle: base62 id (e.g., `0VBJpBmUXk2twqNJdYbHmH`)
  - Season pack ZIP: `sp_{base62}` (e.g., `sp_0VBJpBmUXk2twqNJdYbHmH`)
  - Season pack single episode (uncataloged): `sp_{base62}_ep_{number}` (e.g., `sp_0VBJpBmUXk2twqNJdYbHmH_ep_3`)
  - Season pack single entry (cataloged): `sp_{base62}_entry_{base62}` (e.g., `sp_0VBJpBmUXk2twqNJdYbHmH_entry_7n42DGM5Tflk9n8mt7Fhc7`) — extracts a specific file from the catalog
- **Returns**: SRT file stream (regular/episode) or ZIP archive (season pack)
- **Headers**: Content-Type `text/srt` or `application/zip`, ETag support, file download name
- **Error Codes**:
  - 400: Invalid subtitleId format or missing episode number
  - 404: Subtitle not found or deleted
  - 429: Download rate limit exceeded
- **Cache**: 8 days

#### Get Subtitles (Preferred Endpoint)

```
GET /subtitles/get/{showUniqueId:apiId}/{season:int}/{episode:int}/{language}
```

- **Parameters**:
  - `showUniqueId` (base62-encoded Guid) — from Shows::Search
  - `season` (int, min 0)
  - `episode` (int, min 0)
  - `language` (string, 2+ chars, ISO code)
- **Returns**: `SubtitleSearchResponse` with matching subtitles. When no episode-level subtitles are found, returns season pack entries as fallback:
  - Cataloged packs → one entry per matching file with `sp_{base62}_entry_{base62}` IDs (e.g. regular + dubtitle variants)
  - Uncataloged packs (no entries) → `sp_{base62}_ep_{episode}` ID that extracts the specific episode SRT from the season pack ZIP
- **Error Codes**:
  - 404: Show not found
  - 423: Show is being refreshed, try again later
  - 429: Rate limit

### Media Controller (`/media`)

| Method | Route                                      | Description                                              | Response Cache |
| ------ | ------------------------------------------ | -------------------------------------------------------- | -------------- |
| `GET`  | `/media/trending/{max:range(1,50)}`        | Get trending TV shows/movies                             | 1 day          |
| `GET`  | `/media/{showId:apiId}/details`             | Get media details (poster, overview, etc.)               | 1 day (12 h when not found) |
| `GET`  | `/media/{showId:apiId}/episodes/{language}` | Get last season episodes with subtitles and season packs | 12 hours       |

> Media endpoints set `Cache-Control: public, max-age=...` headers explicitly in the controller rather than via the `[ResponseCache]` attribute.

### Stats Controller (`/stats`)

Provides statistics endpoints for tracking subtitle and show metrics.

### Sitemap Controller

Generates XML sitemaps for SEO purposes.

### Robots Controller (`/robots.txt`)

Serves `robots.txt` from configuration (`text/plain`), replacing the Nuxt static
asset in production (Traefik routes the www host's `robots.txt` here). The
sitemap URL reuses the same base URL as the sitemap generation.

Crawl policy — the download-wall (see
[2026-10-21-llm-discoverability.md](2026-10-21-llm-discoverability.md)):
metadata pages stay open to every agent, subtitle downloads never do.

```
User-Agent: *
Allow: /
Disallow: /subtitles/download/

User-Agent: GPTBot
User-Agent: OAI-SearchBot
User-Agent: ChatGPT-User
User-Agent: PerplexityBot
User-Agent: ClaudeBot
User-Agent: Claude-Web
User-Agent: Google-Extended
User-Agent: CCBot
User-Agent: Bytespider
Allow: /
Disallow: /subtitles/download/

Sitemap: {BaseUrl}/sitemap.xml
# LLM agent guidance: /llms.txt lists the show catalog (metadata only).
```

The AI-crawler agent list is one `AiCrawlerUserAgents` constant on
`RobotsController`, asserted verbatim by `RobotsControllerTests`; the explicit
group exists so agents that only honour their own most-specific block are
covered. New AI crawlers require a code change (one array entry).

### LLM discovery surfaces

- `/llms.txt` (and `/llms-full.txt`, `/sitemap.md`) — generated by the frontend
  (`nuxt-ai-ready`), listing the catalog pages derived from the sitemap.
  Metadata only: no download URI appears in any LLM/SEO-facing surface.
- Show pages expose schema.org JSON-LD (`TVSeries` + `TVSeason` + `TVEpisode`
  nodes) in SSR HTML; episode nodes carry names/season/episode numbers only —
  no download URIs.
- `SubtitlesController.Download` sets `X-Robots-Tag: noindex, nofollow` on
  **every** response path (success, 400, 404, 429) as the first statement of
  the action, so a leaked download URL is never indexed regardless of
  robots.txt compliance.

## Data Transfer Objects (DTOs)

### ShowDto

```csharp
class ShowDto(
    Guid Id,              // UniqueId — serialized as fixed-22-char base62 ("775rJjCDufZ0nuSVDeriR1"), see ID Encoding
    string Name,
    int NbSeasons,        // Total season count
    int[] Seasons,        // Available season numbers
    int? TvDbId,
    int? TmdbId)
{
    string Slug { get; }  // URL-friendly name (computed from Name)
}
```

### SubtitleDto

```csharp
class SubtitleDto
{
    string SubtitleId,     // Subtitle's UniqueId (or sp_-prefixed pack ID)
    string Version,        // Scene/release name for Addic7ed; Release or Filename for packs
    bool Completed,
    bool HearingImpaired,
    bool Corrected,
    bool HD,               // Derived from Qualities (720p/1080p/2160p present)
    string DownloadUri,    // Relative download URL
    string Language,       // English language name
    DateTime Discovered,
    long DownloadCount,
    string Source,         // e.g., "Addic7ed", "SuperSubtitles"
    string[] Qualities,    // e.g., ["720p", "1080p"]
    string? Release        // Full release name (SuperSubtitles; null for Addic7ed)
}
```

### EpisodeDto

```csharp
class EpisodeDto
{
    int Season,
    int Number,
    string Title,
    string Show,           // Show name
    DateTime Discovered
}
```

### EpisodeWithSubtitlesDto

```csharp
class EpisodeWithSubtitlesDto : EpisodeDto
{
    IEnumerable<SubtitleDto> Subtitles
}
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
record struct MediaDetailsDto(ShowDto Media, DetailsDto? Details);

record DetailsDto(
    string PosterPath,
    string Overview,
    string OriginalName,
    MediaType MediaType,   // Show | Movie
    string BackdropPath,
    double VoteAverage,
    string[] Genre,
    string TagLine,
    int? ReleaseYear,
    string EnglishName
);
```

### SeasonPackSubtitleDto

```csharp
record SeasonPackSubtitleDto(
    string SubtitleId,     // sp_-prefixed unique ID
    string Language,
    string Version,        // Release/version name
    string[] ReleaseGroups,// e.g., ["NTb", "FLUX"]
    int? RangeStart,       // First episode in the pack (from pack metadata or cataloged entries)
    int? RangeEnd,         // Last episode in the pack
    string? Uploader,
    DateTime? UploadedAt,
    string[] Qualities,    // e.g., ["720p", "1080p"]
    string Source,         // e.g., "SuperSubtitles"
    string DownloadUri,    // Relative download URL
    long DownloadCount
);
```

### SeasonPackResponse

```csharp
record SeasonPackResponse(
    IEnumerable<SeasonPackSubtitleDto> SeasonPacks
);
```

### TvShowSubtitleResponse (updated)

```csharp
record TvShowSubtitleResponse(
    IAsyncEnumerable<EpisodeWithSubtitlesDto> Episodes,
    IEnumerable<SeasonPackSubtitleDto> SeasonPacks  // Season packs for the season
);
```

### ErrorResponse

```csharp
record ErrorResponse(string Error);
```

## Authentication

The API is **public** — no authentication is required for any endpoint. Rate limiting is applied at the proxy/infrastructure level.

## CORS

Cross-origin requests are governed by a single policy configured in `BootstrapController` (`AddictedProxy/Controllers/Bootstrap/BootstrapController.cs`).

- **Allowed origins** come from the `Cors:AllowedOrigins` array in `appsettings.json`. Entries may use a wildcard subdomain (e.g. `https://*.example.com`), matched via ASP.NET Core's `SetIsOriginAllowedToAllowWildcardSubdomains()`. The scheme is part of the origin, so `http://` variants are rejected unless explicitly listed.
- **Methods & headers**: any method and any request header are allowed.
- **Credentials**: allowed (`AllowCredentials()`), which is why the origin list can never be `*`.
- **Exposed headers**: `Content-Disposition` (subtitle download filename), `sentry-trace` and `baggage` (distributed tracing propagation).
- **Development environment**: any origin is accepted to ease local development.

The list lives **only in configuration** (`appsettings.json` /
`A7D_Cors__AllowedOrigins__*` environment variables) — there is no compiled-in
fallback. An unconfigured deployment denies all origins (fail closed).

Default allowed origins (`appsettings.json`):

| Origin pattern                       | Covers                                    |
| ------------------------------------ | ----------------------------------------- |
| `https://gestdown.info`              | Production frontend (apex)                |
| `https://*.gestdown.info`            | Production frontend subdomains            |
| `https://addictedproxy.pages.dev`    | Cloudflare Pages deployment (apex)        |
| `https://*.addictedproxy.pages.dev`  | Cloudflare Pages preview deployments      |
| `https://subvault.tv`                | SubVault frontend (apex)                  |
| `https://*.subvault.tv`              | SubVault subdomains                       |

To allow a new origin, add it to `Cors:AllowedOrigins` in `appsettings.json` (or via environment variables, e.g. `A7D_Cors__AllowedOrigins__0`) and redeploy.

## Response Caching Strategy

| Content Type            | Cache Duration | Rationale                                    |
| ----------------------- | -------------- | -------------------------------------------- |
| Show search results     | 2 hours        | Show catalog changes infrequently            |
| Media details/trending  | 1 day          | TMDB data is stable                          |
| Media episodes listing  | 12 hours       | Same as subtitle search, longer TTL          |
| Subtitle search results | 2 hours        | New subtitles appear periodically            |
| Season subtitle listing | 2 hours        | Same as subtitle search                      |
| Subtitle file download  | 8 days         | Subtitle content is immutable once completed |
| Not found responses     | 12 hours       | Prevents hammering for non-existent content  |

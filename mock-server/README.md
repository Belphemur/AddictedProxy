# Mock API Server

A lightweight Go HTTP server that emulates the AddictedProxy backend API for local UI development and Playwright testing.

## What It Mocks

| Endpoint | Description |
|---|---|
| `GET /application/info` | App version info |
| `GET /media/trending/{max}` | Trending shows (Breaking Bad, Game of Thrones, Succession) |
| `GET /media/{showId}/details` | Show metadata |
| `GET /media/{showId}/episodes/{language}` | Show details + episodes with subtitles |
| `GET /shows/search/{query}` | Show search results |
| `GET /shows/{showId}/{season}/{language}` | Season episodes with subtitles |
| `POST /shows/{showId}/refresh` | Refresh stub (returns 202) |
| `GET /subtitles/download/{subtitleId}` | Downloads a placeholder `.srt` file |

## Running

```bash
go run .
# or on a custom port:
go run . -port 9090
```

The server listens on `http://localhost:8080` by default.

## Connecting the Frontend

Start the Nuxt dev server with the mock API URL:

```bash
cd ../addicted.nuxt
APP_API_PATH=http://localhost:8080 APP_SERVER_PATH=http://localhost:8080 pnpm dev
```

## Mock Shows

The server includes three hard-coded shows:

| Show | ID | Seasons |
|---|---|---|
| Breaking Bad | `a1b2c3d4-0001-0001-0001-000000000001` | 1–5 |
| Game of Thrones | `a1b2c3d4-0002-0002-0002-000000000002` | 1–8 |
| Succession | `a1b2c3d4-0003-0003-0003-000000000003` | 1–4 |

Episodes are generated dynamically for any requested season/language combination.
Each episode has two subtitle variants: a regular subtitle and a hearing-impaired one,
with alternating sources (`Addic7ed` / `SuperSubtitles`) and quality levels.

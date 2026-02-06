# AddictedProxy

A subtitle search and download proxy for [Addic7ed](https://www.addic7ed.com/), built with .NET 10 and Nuxt 3.

[![.NET](https://github.com/Belphemur/AddictedProxy/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Belphemur/AddictedProxy/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/Belphemur/AddictedProxy/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Belphemur/AddictedProxy/actions/workflows/codeql-analysis.yml)

## Overview

AddictedProxy provides a REST API and web interface for searching and downloading subtitles. It handles upstream communication, caching, compression, and background refresh jobs so clients get fast, reliable access to subtitle data.

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core, Entity Framework Core 10 (PostgreSQL)
- **Frontend:** Nuxt 3, Vue.js 3, Vuetify 3
- **Caching:** Redis + In-Memory
- **Jobs:** Hangfire (PostgreSQL storage)
- **Observability:** OpenTelemetry, Sentry, Prometheus
- **Container:** Docker (Alpine-based)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/) with [pnpm](https://pnpm.io/)
- [Docker](https://www.docker.com/) (for local PostgreSQL)

## Getting Started

### 1. Start the database

```bash
docker compose up -d
```

### 2. Run the backend

```bash
dotnet restore
dotnet run --project AddictedProxy/AddictedProxy.csproj
```

The API will be available with Swagger UI in development mode.

### 3. Run the frontend

```bash
cd addicted.nuxt
pnpm install
pnpm dev
```

## Building

```bash
# Build the full solution
dotnet build -c Release

# Build Docker image
docker build -t addictedproxy .
```

## Testing

```bash
dotnet test -c Release
```

Tests use NUnit 4, NSubstitute for mocking, and FluentAssertions.

## Project Structure

| Project | Description |
|---------|-------------|
| `AddictedProxy` | Main ASP.NET Core web application |
| `AddictedProxy.Database` | EF Core DbContext, entities, and repositories |
| `AddictedProxy.Upstream` | Communication with Addic7ed |
| `AddictedProxy.Caching` | Caching abstractions and implementations |
| `AddictedProxy.Storage` | Storage abstraction (AWS S3) |
| `AddictedProxy.Stats` | Statistics tracking |
| `AddictedProxy.Image` | Image processing |
| `AddictedProxy.Culture` | Culture/language parsing |
| `Compressor` | Zstandard compression utilities |
| `InversionOfControl` | Custom DI bootstrap framework |
| `Locking` | Async keyed locking |
| `Performance` | OpenTelemetry tracing/metrics |
| `ProxyProvider` / `ProxyScrape` | HTTP proxy management |
| `TvMovieDatabaseClient` | TMDB API client |
| `AntiCaptcha` | CAPTCHA solving integration |
| `addicted.nuxt` | Nuxt 3 frontend application |

## Configuration

Environment variables use the `A7D_` prefix. See `appsettings.json` and `appsettings.Development.json` for available settings including connection strings, Redis, rate limiting, and Sentry configuration.

## License

[GPL-3.0](LICENSE)

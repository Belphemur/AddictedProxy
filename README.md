# AddictedProxy

AddictedProxy (Gestdown) is a .NET 10 subtitle platform exposing a public REST API and Nuxt frontend for searching and downloading subtitles.

[![.NET](https://github.com/Belphemur/AddictedProxy/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Belphemur/AddictedProxy/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/Belphemur/AddictedProxy/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Belphemur/AddictedProxy/actions/workflows/codeql-analysis.yml)

## What it does today

- Serves subtitle search and download endpoints used by Gestdown clients.
- Merges subtitle data from multiple providers into a unified show/episode/subtitle model.
- Supports provider-routed downloads (provider selected from subtitle source metadata).
- Runs scheduled and on-demand background jobs for refresh/import pipelines.
- Provides observability via OpenTelemetry, Sentry, and Prometheus-friendly metrics.

## Provider status

- **Addic7ed**: fully integrated for show/season/episode refresh and subtitle download.
- **SuperSubtitles**: integrated through gRPC client and ingestion pipeline:
  - one-time bulk import job
  - recurring incremental refresh job (15-minute cadence)
  - provider-aware subtitle download routing

## Architecture and docs

The `docs/` folder is the source of truth for architecture and subsystem details:

- [Architecture overview](docs/architecture-overview.md)
- [Database schema](docs/database-schema.md)
- [Provider system](docs/provider-system.md)
- [API surface](docs/api-surface.md)
- [Background jobs](docs/background-jobs.md)
- [Multi-provider plan](docs/multi-provider-plan.md)
- [Multi-provider checklist](docs/multi-provider-checklist.md)

## Tech stack

- **Backend:** .NET 10, ASP.NET Core, EF Core 10, PostgreSQL
- **Frontend:** Nuxt 4, Vue 3, Vuetify 3
- **Jobs:** Hangfire + PostgreSQL storage (+ Hangfire.Console)
- **Caching:** PostgreSQL + optional Redis + in-memory
- **Observability:** OpenTelemetry, Sentry, Prometheus
- **Containerization:** Docker / Docker Compose

## Quick start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/) and [pnpm](https://pnpm.io/)
- [Docker](https://www.docker.com/)

### 1) Start local dependencies

```bash
docker compose up -d
```

### 2) Run backend

```bash
dotnet restore
dotnet run --project AddictedProxy/AddictedProxy.csproj
```

### 3) Run frontend

```bash
cd addicted.nuxt
pnpm install
pnpm dev
```

## Build and test

```bash
dotnet build -c Release
dotnet test -c Release
docker build -t addictedproxy .
```

## Configuration

- Settings live in `AddictedProxy/appsettings.json` and `AddictedProxy/appsettings.Development.json`.
- Environment variable prefix is `A7D_`.
- SuperSubtitles import/refresh controls are under `SuperSubtitles:Import` (`EnableImport`, `EnableRefresh`, `BatchSize`, delays).

## Repository highlights

| Project                        | Description                                         |
| ------------------------------ | --------------------------------------------------- |
| `AddictedProxy`                | Main ASP.NET Core app (controllers, jobs, services) |
| `AddictedProxy.Database`       | Entities, EF Core context, repositories, migrations |
| `AddictedProxy.Upstream`       | Addic7ed client/downloader/parser                   |
| `SuperSubtitleClient`          | SuperSubtitles gRPC client                          |
| `AddictedProxy.Services.Tests` | Service-level tests                                 |
| `AddictedProxy.Upstream.Tests` | Upstream integration/unit tests                     |
| `addicted.nuxt`                | Nuxt frontend                                       |

## License

[GPL-3.0](LICENSE)

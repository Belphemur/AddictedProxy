# Copilot Instructions for AddictedProxy

## Project Overview

AddictedProxy is a .NET 10 ASP.NET Core application that provides a proxy API for searching and downloading subtitles from Addic7ed. It includes a Nuxt 4 (Vue.js) frontend, PostgreSQL database, background job scheduling with Hangfire, and comprehensive observability via OpenTelemetry and Sentry.

**License:** GPL-3.0

## Repository Structure

```
AddictedProxy/              # Main ASP.NET Core web application (entry point)
AddictedProxy.Caching/      # Caching abstractions and implementations
AddictedProxy.Culture/      # Culture/language parsing utilities
AddictedProxy.Database/     # EF Core DbContext, entities, repositories (PostgreSQL)
AddictedProxy.Image/        # Image processing with ImageSharp
AddictedProxy.OneTimeMigration/ # One-time data migration utilities
AddictedProxy.Stats/        # Statistics tracking
AddictedProxy.Storage/      # Storage abstraction (AWS S3)
AddictedProxy.Storage.Caching/ # Cached storage layer
AddictedProxy.Tools.Database/  # Database tooling helpers
AddictedProxy.Upstream/     # Upstream Addic7ed communication
AddictedProxy.Upstream.Tests/  # Tests for upstream module
AntiCaptcha/                # CAPTCHA solving integration
AntiCaptcha.Tests/          # Tests for AntiCaptcha
Compressor/                 # Zstandard compression utilities
Compressor.Tests/           # Tests for compressor
InversionOfControl/         # Custom DI bootstrap framework
InversionOfControl.Tests/   # Tests for IoC
Locking/                    # Async keyed locking utilities
Performance/                # OpenTelemetry tracing/metrics
ProxyProvider/              # HTTP proxy provider abstraction
ProxyProvider.Tests/        # Tests for proxy provider
ProxyScrape/                # Proxy scraping implementation
TvMovieDatabaseClient/      # TMDB API client
addicted.nuxt/              # Nuxt 4 frontend (Vue.js + Vuetify)
```

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 10.0, ASP.NET Core |
| Database | PostgreSQL 18 via EF Core 10 + Npgsql |
| Caching | PostgreSQL (primary) + Redis (optional) + In-Memory |
| Jobs | Hangfire with PostgreSQL storage |
| Observability | OpenTelemetry, Sentry, Prometheus |
| Compression | ZstdSharp.Port |
| Frontend | Nuxt 4, Vue.js 3, Vuetify 3, pnpm |
| Testing | NUnit 4, NSubstitute, FluentAssertions |
| CI/CD | GitHub Actions, semantic-release |
| Container | Docker (Alpine-based), Docker Compose |

## Build & Development Commands

### Backend (.NET)

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build -c Release

# Run all tests
dotnet test -c Release

# Run the main application
dotnet run --project AddictedProxy/AddictedProxy.csproj

# Start local PostgreSQL via Docker Compose
docker compose up -d
```

### Frontend (Nuxt)

```bash
cd addicted.nuxt
pnpm install
pnpm dev
```

### Docker

```bash
docker build -t addictedproxy .
```

## Architecture & Key Patterns

### Bootstrap Pattern (Dependency Injection)

The project uses a custom DI bootstrap system in the `InversionOfControl` project. Each module registers its services by implementing `IBootstrap` (for service registration) and/or `IBootstrapApp` (for middleware/app configuration).

```csharp
// Registration in Program.cs
builder.AddBootstrap(typeof(BootstrapDatabase).Assembly, typeof(BootstrapController).Assembly, ...);
app.UseBootstrap(typeof(BootstrapDatabase).Assembly, typeof(BootstrapController).Assembly, ...);
```

**When adding a new module:** Create a class implementing `IBootstrap` in the module project. It will be discovered by assembly scanning in `Program.cs`. If the module's assembly isn't already passed to `AddBootstrap`/`UseBootstrap`, add it there.

Conditional bootstrapping is supported via `IBootstrapConditional` (checked at registration time) and environment variable parsing via `IBootstrapEnvironmentVariable<T, TParser>`.

### Database

- **DbContext:** `EntityContext` in `AddictedProxy.Database`
- **Entities:** `TvShow`, `Season`, `Episode`, `Subtitle`, `AddictedUserCredentials`
- **Repositories:** Follow repository pattern in `AddictedProxy.Database/Repositories/`
- **Migrations:** Auto-applied on application startup
- **IDs:** Sortable GUIDs via RT.Comb

### API Controllers

Controllers are in `AddictedProxy/Controllers/Rest/` and use:
- Attribute routing (`[Route("...")]`)
- ASP.NET Core's built-in `IResult` with `TypedResults` and `Results<T1, T2, ...>` for new endpoints
  - See [Action return types](https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types)
  - Some legacy endpoints still use `Ardalis.Result` — prefer `TypedResults` for new code
- Response caching
- XML documentation (documentation file generation is enabled)

### Configuration

- Environment variables use `A7D_` prefix convention
- Settings files: `appsettings.json` and `appsettings.Development.json`
- Key config sections: connection strings, PostgreSQL caching, rate limiting, proxy scraping, Sentry

### Centralized Package Management

All NuGet package versions are managed centrally in `Directory.Packages.props` at the solution root. Project `.csproj` files reference packages without version numbers. **Always update versions in `Directory.Packages.props`**, not in individual project files.

## Testing Guidelines

- **Framework:** NUnit 4 with `NUnit3TestAdapter`
- **Mocking:** NSubstitute
- **Assertions:** FluentAssertions
- **Coverage:** coverlet.collector
- Test projects follow the `{ProjectName}.Tests` naming convention
- Run tests: `dotnet test -c Release`

## Code Style & Conventions

- **Nullable reference types** enabled globally
- **Implicit usings** enabled globally
- **C# latest** language version (via .NET 10 SDK)
- No `.editorconfig` — follow existing code patterns
- XML documentation comments on public API surfaces (controllers, public services)
- `record` types used for DTOs
- Async/await throughout with `CancellationToken` support
- Dependency injection everywhere — no service locator pattern

## CI/CD Notes

- GitHub Actions workflow in `.github/workflows/dotnet.yml`
- Restore uses `dotnet restore` (no lock files present currently)
- Semantic versioning via `semantic-release` (npm package at root)
- Docker images published to GitHub Container Registry (ghcr.io)
- Build provenance attestation enabled
- CodeQL analysis configured for C# and JavaScript

## Common Tasks

### Adding a new NuGet package

1. Add the version to `Directory.Packages.props`
2. Add the `<PackageReference>` (without version) to the relevant `.csproj`

### Adding a new project

1. Create the project: `dotnet new classlib -n ProjectName`
2. Set `<TargetFramework>net10.0</TargetFramework>` with `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`
3. Add it to `AddictedProxy.sln`
4. If it has a bootstrap class, add its assembly reference to `Program.cs`

### Adding a new API endpoint

1. Create or extend a controller in `AddictedProxy/Controllers/Rest/`
2. Use `[ApiController]` and `[Route("...")]` attributes
3. Return ASP.NET Core `Results<Ok<T>, NotFound, ...>` with `TypedResults.Ok()`, `TypedResults.NotFound()`, etc. for structured responses
4. Add XML documentation for Swagger

### Committing

**ALWAYS use Conventional Commits format for all commit messages.**

Follow the [Conventional Commits specification](https://www.conventionalcommits.org/):

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

#### Commit Types

- **feat**: A new feature for the user
  - Example: `feat(api): add endpoint for subtitle search by IMDB ID`
- **fix**: A bug fix
  - Example: `fix(database): resolve connection timeout issue in EF Core`
- **docs**: Documentation only changes
  - Example: `docs(readme): update installation instructions`
- **style**: Changes that don't affect code meaning (formatting, whitespace, etc.)
  - Example: `style(controllers): fix indentation in SubtitlesController`
- **refactor**: Code change that neither fixes a bug nor adds a feature
  - Example: `refactor(caching): simplify cache key generation logic`
- **perf**: Performance improvements
  - Example: `perf(database): add index on subtitle language column`
- **test**: Adding or updating tests
  - Example: `test(upstream): add tests for Addic7ed scraper`
- **build**: Changes to build system or dependencies
  - Example: `build(deps): update Npgsql to 10.0.1`
- **ci**: Changes to CI configuration files and scripts
  - Example: `ci(github): add CodeQL security scanning`
- **chore**: Other changes that don't modify src or test files
  - Example: `chore(gitignore): add VS Code workspace files`

#### Scopes (Optional but Recommended)

Common scopes in this project:
- `api`, `controllers`, `services`
- `database`, `caching`, `storage`
- `upstream`, `proxy`, `captcha`
- `frontend`, `nuxt`
- `deps`, `docker`, `ci`

#### Breaking Changes

For breaking changes, add `!` after type/scope and include `BREAKING CHANGE:` in footer:

```
feat(api)!: change subtitle search response format

BREAKING CHANGE: SubtitleSearchResponse now returns array instead of object
```

#### Examples

```
feat(controllers): migrate all endpoints to ASP.NET Core Results types

Replace Ardalis.Result with built-in Results<T1, T2, ...> for better type safety
and improved OpenAPI documentation.

fix(database): handle null reference in episode repository

Fixes #123

docs(copilot): add Conventional Commits guidelines

chore(deps): update all NuGet packages to latest versions
```

#### Rules

1. Use lowercase for type, scope, and description
2. Keep description under 72 characters
3. Use imperative mood ("add" not "added" or "adds")
4. No period at the end of description
5. Body and footer are optional but recommended for complex changes

### Adding a new background job

1. Create a job class in the relevant module
2. Register it via the module's `IBootstrap` implementation
3. Schedule via Hangfire's `IRecurringJobManager` or `IBackgroundJobClient`

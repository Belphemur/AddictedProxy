# AGENTS.md

Canonical agent instructions for the **AddictedProxy** repository
(`Belphemur/AddictedProxy` — .NET 10 ASP.NET Core + Nuxt 4). License: GPL-3.0.

This file is the single source of truth for how work gets done here. It is
read by Copilot, Codex, Cursor, Claude, and any other agent that supports the
`AGENTS.md` standard. The companion `.agents/skills/` directory holds
reusable, task-specific procedures (Agent Skills spec).

## Table of contents

- [1. Before you write code](#1-before-you-write-code) — mandatory loading order
- [2. Repository layout](#2-repository-layout) — what exists, what each module is
- [3. Build & develop](#3-build--develop) — restore/build/test commands
- [4. Testing](#4-testing) — unit + e2e conventions and gates
- [5. Commits & pull requests](#5-commits---pull-requests) — Conventional Commits, workflow
- [6. Docs & design](#6-docs--design) — index of `docs/`
- [7. Skills reference](#7-skills-reference) — `.agents/skills/` index

---

## 1. Before you write code

1. **Load the `coding-philosophy` skill first.** It is marked `mandatory: true`
   in `.agents/skills/coding-philosophy/SKILL.md` and encodes the repo's
   decision framework: **DRY > SOLID > KISS** (strict priority). Resolve every
   design choice against it and name the trade-off in the commit/PR.
2. **Consult the docs.** Read `docs/architecture-overview.md`,
   `docs/provider-system.md`, and the relevant domain doc *before* making
   architectural changes — see the [docs index](#6-docs--design).
3. **Evaluate installed skills** against the task. For each skill in
   `.agents/skills/`, decide YES/NO relevance and invoke **all** YES skills
   before proceeding. (Mirrors the `skilld` contract.)
4. **Design doc before code.** Non-trivial or API-visible changes need a short
   design note (an ADR under `docs/adr/` or a pinned section in the PR) stating
   the chosen approach and the rejected alternatives. Small fixes may skip
   this at the implementer's judgment.

> **No code changes without tests.** Every production change must add or update
> coverage. See [Testing](#4-testing).

---

## 2. Repository layout

```
AddictedProxy/        ASP.NET Core web app (entry point, controllers, middleware)
AddictedProxy.Caching/       Caching abstractions + Redis/in-memory impls
AddictedProxy.Cloudflare.Worker/  Cloudflare Worker glue
AddictedProxy.Culture/       Culture/language parsing utilities
AddictedProxy.Database/     EF Core DbContext, entities, repos, migrations (Postgres)
AddictedProxy.Image/         ImageSharp processing
AddictedProxy.OneTimeMigration/  One-time data migration utilities
AddictedProxy.Resilience.Tests/   Resilience pipeline tests
AddictedProxy.Services/        Service layer (shown in structure)
AddictedProxy.Services.Tests/     Service-layer + app tests
AddictedProxy.Stats/            Statistics tracking
AddictedProxy.Storage/          Storage abstraction (S3)
AddictedProxy.Storage.Caching/  Cached storage layer
AddictedProxy.Tools.Database/   DB tooling helpers
AddictedProxy.Upstream/         Upstream Addic7ed + SuperSubtitles gRPC clients
AddictedProxy.Upstream.Tests/
AntiCaptcha/ + AntiCaptcha.Tests/
Compressor/ + Compressor.Tests/       Zstd wrapper
InversionOfControl/ + .Generator/ + Tests/  Custom DI bootstrap + source generator
Locking/                  Async keyed lock utilities
Performance/              OpenTelemetry + Prometheus
ProxyProvider/ + Tests/    HTTP proxy provider abstraction
ProxyScrape/              Proxy scraping
SuperSubtitleClient/      gRPC client + import/refresh jobs for SuperSubtitles
TvMovieDatabaseClient/    TMDB API client
addicted.nuxt/            Nuxt 4 frontend (Vue 3, Vuetify 4, Pinia, Tailwind)
mock-server/              Go mock API + SignalR for local UI dev & e2e
docs/                     Architecture & design docs (authoritative)
.github/workflows/        CI/CD (ci-cd.yml + reusable workflows)
.agents/skills/           Repo agent skills (Agent Skills spec)
compose.yaml              Postgres 18 + real backend dev stack
docker-compose.dev.yml    Mock API + production-built Nuxt frontend (UI dev)
Dockerfile, addicted.nuxt/Dockerfile  Production images
```

Notes:
- `.NET` — central package management via `Directory.Packages.props`; no per-project versions.
- `InversionOfControl.Generator` is registered as an analyzer in `Directory.Build.props` and runs on every non-generator project — be aware of generated-code diagnostics.
- New ASP.NET Core web API endpoints return `TypedResults`/`Results<T1,T2,...>` (not `Ardalis.Result`).

---

## 3. Build & develop

```bash
# .NET backend
dotnet restore
dotnet build -c Release                                     # all projects incl. IoC generator
dotnet test  -c Release                                     # runs over the whole solution

# Frontend
cd addicted.nuxt
pnpm install
pnpm dev                    # against mock: APP_API_PATH=http://localhost:8080 pnpm dev
pnpm build                  # production build (required before Docker/dev-compose)

# Mock stack (no DB/backend needed for UI work)
docker compose -f docker-compose.dev.yml up --build
# or native: cd mock-server && go run .   (Go 1.26)

# Real backend dev stack (Postgres + SuperSubtitles)
docker compose up -d
```

- Frontend env vars: `A7D_*` (backend) and `NUXT_PUBLIC_*` (runtime URL overrides) — see appsettings and `docs/database-schema.md`.

---

## 4. Testing

There are **two** test surfaces; both must stay green on every PR.

### Unit tests (.NET)

- **Stack:** NUnit 4 (`NUnit3TestAdapter`), NSubstitute, FluentAssertions, coverlet.
- **Projects:** 7 `*.Tests` projects — one per major module
  (`AddictedProxy.Upstream.Tests`, `AddictedProxy.Services.Tests`,
  `AddictedProxy.Resilience.Tests`, `InversionOfControl.Tests`,
  `Compressor.Tests`, `AntiCaptcha.Tests`, `ProxyProvider.Tests`).
- **Where tests live:** `Services.Tests` and `Resilience.Tests` reference the full app and cross-cutting concerns, so a change to shared DI/resilience/config code is gated by the broadest test projects — run those first when a unit failure is suspected.
- **Skill:** `.agents/skills/backend-testing/SKILL.md` — conventions, fixture patterns, and which project hosts tests for which module.
- **CI gate:** `reusable-dotnet.yml` runs `dotnet test -c Release --no-restore` over the **whole solution**; the `CI Gate` in `ci-cd.yml` requires it.

Conventions:
- `MethodName_Condition_ExpectedResult` naming; Arrange–Act–Assert, one behavior per test.
- Mock only external boundaries (HTTP clients, `EntityContext`/repositories, upstream APIs, storage). Prefer state-based assertions over behavior verification.
- Async: `async Task` + `CancellationToken` where the SUT supports it.

### E2E tests (Nuxt / Playwright)

- **Location:** `addicted.nuxt/e2e/` (`playwright.config.ts` discovers `*.spec.ts`).
- **Always run against the mock server** (`mock-server/`, Go) — never a real backend. See `mock-environment-setup` skill.
- **Projects:** chromium + iPhone 13 (mobile UA detected server-side — mobile tests need a fresh context with UA override).
- **CI gate:** `reusable-nuxt-e2e.yml` boots the mock server, builds a production Nuxt output, installs `ws` into `.output/server`, runs `playwright test`; aggregated by `ci-cd.yml` "CI Gate".
- **Skill:** `.agents/skills/e2e-test-authoring/SKILL.md`.

Visual/UI changes: screenshot + verify glass-panel styling and hydration-mismatch-free rendering on desktop and mobile before pushing.

---

## 5. Commits & pull requests

- **Conventional Commits**: `<type>[scope]: <imperative description>`, optional body,
  `BREAKING CHANGE:` footer for breaking changes.
- Types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`.
- Default branch is `dev`; PRs target `dev`. `main` is release-only.
- **Workflow:** design → implement + tests green locally → open PR → CI green → review →
  squash-merge with a **user-facing changelog title** → on `main`/release, cut a GitHub Release.
- No `Co-Authored-By` trailers.

---

## 6. Docs & design

The `docs/` folder is authoritative. The [coding-philosophy](#1-before-you-write-code)
audit rule applies: when refactoring, verify each doc describes reality, not
aspiration.

| Document | Scope |
|---|---|
| `architecture-overview.md` | Tech stack, data flow, bootstrap (IoC) pattern, resilience, key types |
| `database-schema.md` | Entities, indexes, enums, repositories, migration strategy |
| `provider-system.md` | Addic7ed + SuperSubtitles providers, refresher pipeline, credentials |
| `api-surface.md` | REST endpoints, DTOs, response caching, download formats |
| `background-jobs.md` | Hangfire jobs/queues, one-time migrations, attributes |
| `frontend-ux-design.md` | Glass-panel design system, Vuetify 4, composables, Playwright |
| `multi-provider-plan.md` | SuperSubtitles provider design + status |
| `multi-provider-checklist.md` | Provider integration checklist |
| `season-pack-plan.md` | Season pack feature design |
| `season-pack-catalog-plan.md` | Season pack catalog design |
| `season-pack-catalog-checklist.md` | Season pack catalog implementation checklist |

---

## 7. Skills reference

Repo skills live in `.agents/skills/` (Agent Skills spec; also discovered at
`.github/skills/` for backward compatibility, but **new skills go in `.agents/skills/`**).
`skilld` manages the third-party `nuxt-scripts-skilld`; its lock file is
`.agents/skills/skilld-lock.yaml`. **Note:** `skilld prepare` may regenerate
outputs into `.github/skills/` by default — if that happens, move/symlink them
into `.agents/skills/` and commit.

| Skill | Scope | When to load |
|---|---|---|
| `coding-philosophy` | DRY > SOLID > KISS decision framework | **Mandatory — before writing or reviewing any code** |
| `backend-testing` | NUnit/NSubstitute/FluentAssertions conventions, test-project mapping | When writing/fixing .NET unit tests |
| `ef-core-migrations` | Creating/applying/troubleshooting EF Core migrations | When changing the schema |
| `e2e-test-authoring` | Playwright e2e for the Nuxt frontend | When writing/fixing frontend e2e |
| `mock-environment-setup` | Docker Compose mock stack for UI dev/e2e | When the mock API is needed |
| `nuxt-scripts-skilld` | `@nuxt/scripts` usage/troubleshooting | When touching Nuxt telemetry scripts |

<!-- skilld -->
Before modifying code, evaluate each installed skill against the current task.
For each skill, determine YES/NO relevance and invoke all YES skills before proceeding.
<!-- /skilld -->

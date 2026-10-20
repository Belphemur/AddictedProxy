# AGENTS.md

Canonical agent instructions for the **AddictedProxy** repository
(`Belphemur/AddictedProxy` — .NET 10 ASP.NET Core + Nuxt 4). License: GPL-3.0.

This file is an **index and policy document**, not a procedure manual. It names
what must be true and where to find the *how*. Procedural detail lives in the
reusable skills under [`.agents/skills/`](#7-skills-reference) and the domain docs
under [`docs/`](#6-docs--design); AGENTS.md is the single place an agent
(human, Copilot, Cursor, Claude, Codex, etc.) checks first and the only thing
that must stay in sync with the repo on every change.

---

## Table of contents

1. [Before you write code](#1-before-you-write-code) — load order (mandatory)
2. [Repository layout](#2-repository-layout) — projects and what each holds
3. [Build & develop](#3-build--develop) — restore/build/test/run commands
4. [Testing](#4-testing) — policy + unit/e2e index (both gate every PR)
5. [Commits & pull requests](#5-commits--pull-requests) — Conventional Commits + workflow
6. [Docs & design](#6-docs--design) — `docs/` index + design-before-code rule
7. [Skills reference](#7-skills-reference) — `.agents/skills/` index + mandatory markers
8. [Agent-readiness baseline](#8-agent-readiness-baseline) — what makes this repo agent-ready

---

## 1. Before you write code

The order below is enforced, not advisory. Skipping step 1 is a review-failing
trade-off (a **KISS-over-SOLID** correctness hole — see
[coding-philosophy](#11-load-the-mandatory-skill-coding-philosophy-first)).

1. **Load the mandatory skill: `coding-philosophy`.** Read
   `.agents/skills/coding-philosophy/SKILL.md`. It is the repo's decision
   framework — **DRY > SOLID > KISS** in strict priority order — and is marked
   `metadata.mandatory: true`. It **must be loaded before writing, editing,
   reviewing, or planning any code, with no exceptions.** Every non-trivial
   design decision and its winning trade-off **must be named in the commit
   message or PR description.** If a dependency bump makes local code
   redundant, delete the local copy and consume the SDK's.

2. **Consult the docs.** Read `docs/architecture-overview.md`,
   `docs/provider-system.md`, the relevant domain doc, and
   `docs/database-schema.md` before an architectural change. See
   [the docs index](#6-docs--design). Undocumented behaviour is a smell — fix
   the doc in the same change that fixes the behaviour.

3. **Evaluate installed skills.** For each skill in `.agents/skills/`, decide
   YES/NO relevance and invoke **all YES skills** before proceeding. This is the
   skilld contract:

   <!-- skilld -->
   Before modifying code, evaluate each installed skill against the current task.
   For each skill, determine YES/NO relevance and invoke all YES skills before proceeding.
   <!-- /skilld -->

4. **Design doc before code.** Non-trivial or API-visible changes need a short
   design note — a pinned section in the PR body or, for reusable decisions, a
   `docs/*-plan.md` entry — stating the chosen approach and the rejected
   alternatives (name the losing principle). Small fixes may skip this at the
   implementer's judgment.

> **No code changes without tests.** Every production change must add or update
> coverage. See [Testing](#4-testing).

### 1.1 Load the mandatory skill `coding-philosophy` first

`.agents/skills/coding-philosophy/SKILL.md` exists in-repo and is the source of
truth (not any personal/profile copy). Its priority order:

1. **DRY wins over SOLID.** One source of truth per behaviour/format/constant;
   refactor duplicate paths into one funnel before writing new code beside them.
2. **SOLID wins over KISS.** Fail closed; never corrupt the only copy of data;
   no two writers racing on one resource (episode row, credential, storage
   object, keyed lock, Hangfire idempotency).
3. **KISS breaks ties within DRY.** Once knowledge lives once, the simplest
   structure that satisfies the design wins; delete machinery whose job vanished.

Repo-specific applications (full text in the skill): central package management
(`Directory.Packages.props` — never inline a version in a `.csproj`); the
`IBootstrap` bootstrap as the single DI funnel; `TypedResults`/`Results<T1,T2>`
for new REST endpoints (do not revive legacy `Ardalis.Result`); raw SQL lives
only for performance and **must be updated in lock-step with the columns it
touches** (`EpisodeRepository.MergeEpisodeWithSubtitleAsync`).

---

## 2. Repository layout

```
AddictedProxy/              ASP.NET Core web app: entry point, REST controllers,
                            Program.cs, Services/, Model/, Migrations/ (one-time)
AddictedProxy.Caching/      Caching abstractions + Redis/in-memory impls
AddictedProxy.Cloudflare.Worker/  Cloudflare Worker glue
AddictedProxy.Culture/      Culture/language parsing utilities
AddictedProxy.Database/     EF Core (Postgres 18 + Npgsql 10) DbContext, entities,
                            repos, schema migrations (auto-applied at startup)
AddictedProxy.Image/        ImageSharp processing (v4, Six Labors community license)
AddictedProxy.OneTimeMigration/  One-time data-migration framework
AddictedProxy.Resilience.Tests/   Polly resilience-pipeline tests
AddictedProxy.Services.Tests/     Service layer + app tests (references AddictedProxy)
AddictedProxy.Stats/        Statistics tracking
AddictedProxy.Storage/      Storage abstraction (S3)
AddictedProxy.Storage.Caching/    Cached storage layer
AddictedProxy.Tools.Database/     DB tooling helpers
AddictedProxy.Upstream/     Upstream Addic7ed client
AddictedProxy.Upstream.Tests/
AntiCaptcha/ + AntiCaptcha.Tests/    CAPTCHA solving (DORMANT — not registered in
                                       Program.cs bootstrap, preserved for reuse)
Compressor/ + Compressor.Tests/      Zstd wrapper (ZstdSharp.Port)
InversionOfControl/ + .Generator/ + Tests/  Custom DI bootstrap + source generator
Locking/                  Async keyed-lock utilities
Performance/              OpenTelemetry + Prometheus + Sentry
ProxyProvider/ + Tests/    HTTP proxy provider abstraction
ProxyScrape/              Residential-proxy quota (ProxyScrape v4 API)
SuperSubtitleClient/      gRPC client + import/refresh jobs for SuperSubtitles
TvMovieDatabaseClient/    TMDB API client
addicted.nuxt/            Nuxt 4 frontend (Vue 3, Vuetify 4, Pinia, Tailwind, pnpm)
mock-server/              Go mock API + SignalR for local UI dev & e2e
docs/                     Architecture & design docs (authoritative)
.github/workflows/        CI/CD (ci-cd.yml + reusable workflows)
.agents/skills/           Repo agent skills (Agent Skills spec)
compose.yaml              Postgres 18 + real backend dev stack
docker-compose.dev.yml    Mock API + production-built Nuxt frontend (UI dev)
Dockerfile, addicted.nuxt/Dockerfile  Production images
```

Notes:
- **.NET** — central package management via `Directory.Packages.props`; no per-project versions. Target `net10.0`.
- `AddictedProxy/` owns its service layer under `AddictedProxy/Services/` (there is **no** separate `AddictedProxy.Services` project).
- `InversionOfControl.Generator` is registered as an analyzer in `Directory.Build.props` and runs on every non-generator project — expect generated-code diagnostics.
- New REST endpoints return `TypedResults`/`Results<T1,T2,...>` (not `Ardalis.Result`).
- `AntiCaptcha/` is dormant: no assembly is passed to `AddBootstrap`/`UseBootstrap` in `Program.cs`.

---

## 3. Build & develop

```bash
# .NET backend (solution-wide, what CI mirrors)
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

# Real backend dev stack (Postgres 18 + SuperSubtitles)
docker compose up -d          # compose.yaml at repo root
```

- Frontend env vars: `A7D_*` (backend) and `NUXT_PUBLIC_*` (runtime URL overrides) — see appsettings and `docs/database-schema.md`.

---

## 4. Testing

There are **two** test surfaces; both must stay green on every PR.

| Surface | What | Tooling | Gate | Full conventions |
|---|---|---|---|---|
| Unit | .NET | NUnit 5 + NSubstitute + FluentAssertions + coverlet, `net10.0` | `reusable-dotnet.yml` | [backend-testing skill](.agents/skills/backend-testing/SKILL.md) |
| E2E | Nuxt | Playwright (chromium + iPhone 13) | `reusable-nuxt-e2e.yml` | [e2e-test-authoring skill](.agents/skills/e2e-test-authoring/SKILL.md) |

### Unit tests (.NET)

- **Stack:** NUnit 5 (`NUnit3TestAdapter`), NSubstitute, FluentAssertions, coverlet — versions pinned once in `Directory.Packages.props`.
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

| Skill | Scope | When to load | Mandatory |
|---|---|---|---|
| `coding-philosophy` | DRY > SOLID > KISS decision framework | **Before writing or reviewing any code — no exceptions** | ✅ yes |
| `backend-testing` | NUnit/NSubstitute/FluentAssertions conventions, test-project mapping | When writing/fixing .NET unit tests | no |
| `ef-core-migrations` | Creating/applying/troubleshooting EF Core migrations | When changing the schema | no |
| `e2e-test-authoring` | Playwright e2e for the Nuxt frontend | When writing/fixing frontend e2e | no |
| `mock-environment-setup` | Docker Compose mock stack for UI dev/e2e | When the mock API is needed | no |
| `nuxt-scripts-skilld` | `@nuxt/scripts` usage/troubleshooting | When touching Nuxt telemetry scripts | no |

<!-- skilld -->
Before modifying code, evaluate each installed skill against the current task.
For each skill, determine YES/NO relevance and invoke all YES skills before proceeding.
<!-- /skilld -->

---

## 8. Agent-readiness baseline

This repo is skill-driven and CI-gated by evidence, not intent:

- **Mandatory-skill loader.** The skilld contract above + `coding-philosophy`'s
  `metadata.mandatory: true` together force evaluation of every installed skill
  and the unconditional load of `coding-philosophy` before any code task.
- **Whole-solution gates.** `dotnet test -c Release --no-restore` over the
  **whole solution** (not per-project) in CI; a build is clean only at
  `0 errors / 0 warnings` — fixes belong in code, never in suppressions.
- **Formatting baseline.** A repo-root `.editorconfig` (and one in
  `addicted.nuxt/`) standardises indent/charset/EOL; Roslyn analyzer
  diagnostics stay advisory so adopting conventions never breaks a build or
  forces a repo-wide reformat.
- **Central package management.** One version per package in
  `Directory.Packages.props`; version drift between the manifest and the docs
  is a review-finding.
- **Copilot.** GitHub Copilot reads `AGENTS.md` natively;
  `.github/copilot-instructions.md` is a backward-compatibility pointer to it
  and must not diverge from it.

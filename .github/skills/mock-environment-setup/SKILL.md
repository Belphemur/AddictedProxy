---
name: mock-environment-setup
description: Step-by-step guide for spinning up the Docker Compose mock environment to develop and test the Nuxt frontend without a real .NET backend or database. Use this skill whenever asked to start, configure, or troubleshoot the local mock dev stack.
---

The AddictedProxy repository ships a self-contained development stack in
`docker-compose.dev.yml` that wires together two containers:

| Service    | URL                     | Description                                                                   |
| ---------- | ----------------------- | ----------------------------------------------------------------------------- |
| `mock-api` | `http://localhost:8080` | Go mock server — emulates every REST + SignalR endpoint the frontend consumes |
| `frontend` | `http://localhost:3000` | Production-built Nuxt 4 app pointed at the mock API                           |

No real .NET backend, PostgreSQL database, or external credentials are required.

---

## 1 — Prerequisites

- **Docker Desktop** (or Docker Engine + Compose plugin) installed and running.
- Repository checked out locally.

---

## 2 — Start the full stack

From the repository root run:

```bash
docker compose -f docker-compose.dev.yml up --build
```

The `--build` flag re-builds both images from source so any recent code changes
are picked up. On subsequent starts you can omit `--build` if neither the
mock server nor the frontend source has changed:

```bash
docker compose -f docker-compose.dev.yml up
```

Once both services report healthy, open:

- **Frontend:** <http://localhost:3000>
- **Mock API:** <http://localhost:8080>

---

## 3 — How it works

### Mock API server (`mock-server/`)

- Written in Go; built with a multi-stage Dockerfile that produces a static
  binary in a `scratch` image.
- Listens on port **8080** inside the container (mapped to `8080` on the host).
- All mock data lives in **`mock-server/data/`** — edit these JSON files to add
  shows, change episode titles, or tweak subtitle generation without touching Go:

  | File          | Purpose                                                               |
  | ------------- | --------------------------------------------------------------------- |
  | `shows.json`  | Show definitions, episode titles, TMDB details, `seasonPackOnly` flag |
  | `config.json` | App version, episode subtitle cycles, season pack template values     |

- Serves four shows (defined in `data/shows.json`):

  | Show             | ID                                     | Seasons | Notes                                        |
  | ---------------- | -------------------------------------- | ------- | -------------------------------------------- |
  | Breaking Bad     | `a1b2c3d4-0001-0001-0001-000000000001` | 1–5     | Full episodes + season packs                 |
  | Game of Thrones  | `a1b2c3d4-0002-0002-0002-000000000002` | 1–8     | Full episodes + season packs                 |
  | Succession       | `a1b2c3d4-0003-0003-0003-000000000003` | 1–4     | Full episodes + season packs                 |
  | Only Season Pack | `a1b2c3d4-0004-0004-0004-000000000004` | 1–2     | **Season packs only — no episode subtitles** |

- Each regular show episode returns two subtitle variants (regular + hearing-impaired)
  with alternating `Addic7ed` / `SuperSubtitles` sources and quality chips.
- **Only Season Pack** is a dedicated test show that returns zero episodes and only
  season pack subtitles. Use it to verify UI behaviour when no per-episode subtitles
  exist (e.g. confirming the "Episodes" header is hidden).
- SignalR connections (`/refresh`) are accepted and held open; no hub events
  are emitted.

### Test case: season-pack-only show

Navigate to the _Only Season Pack_ show page to verify the "Episodes" header and
divider are **not** rendered when a season has only season packs:

```
/shows/a1b2c3d4-0004-0004-0004-000000000004/only-season-pack
```

Expected: Season Packs section is visible; "Episodes" heading and the horizontal
divider above it are **absent**.

### Nuxt frontend (`addicted.nuxt/`)

The dev stack builds the frontend using `addicted.nuxt/Dockerfile.dev` (a
lightweight variant of the production `Dockerfile` that requires no Alpine
package registry access). It uses production presets (`NUXT_PRESET=node-server`)
and overrides API URLs at runtime via Nuxt public runtime variables:

| Variable                            | Value in dev stack      | Purpose                            |
| ----------------------------------- | ----------------------- | ---------------------------------- |
| `NUXT_PUBLIC_API_CLIENT_URL`        | `http://localhost:8080` | Browser-side fetch target          |
| `NUXT_PUBLIC_API_SERVER_URL`        | `http://mock-api:8080`  | SSR fetch target (Docker hostname) |
| `NUXT_PUBLIC_URL`                   | `http://localhost:3000` | Canonical frontend URL             |
| `NUXT_PUBLIC_SENTRY_CONFIG_ENABLED` | `false`                 | Disable Sentry in the dev stack    |

This differs from native local development. When you run the Nuxt app directly
with `pnpm dev`, this repository currently uses the unprefixed `APP_*`
variables consumed in `nuxt.config.ts`.

---

## 4 — Alternative: native Nuxt dev server

If you prefer hot-reload during UI development, run the Go mock server
natively and start the Nuxt dev server against it:

```bash
# Terminal 1 — start the mock API
cd mock-server
go run .          # listens on :8080; use -port NNNN to override

# Terminal 2 — start the Nuxt dev server (MUST be in addicted.nuxt/)
cd addicted.nuxt
APP_URL=http://localhost:3000 APP_API_PATH=http://localhost:8080 APP_SERVER_PATH=http://localhost:8080 SENTRY_ENABLE=false pnpm dev
```

> **Important:** All `pnpm` commands (`pnpm dev`, `pnpm install`, `pnpm exec playwright test`, etc.)
> must be run from the `addicted.nuxt/` directory — that is where `package.json` lives.

Nuxt dev server runs on <http://localhost:3000> with HMR enabled.

Summary:

- Docker Compose dev stack: use `NUXT_PUBLIC_*`
- Native local `pnpm dev`: use `APP_*`

---

## 5 — Playwright testing against the mock stack

The mock server is the correct backend for all Playwright tests. Start the
stack first (either via Docker Compose or natively), then run from the
`addicted.nuxt/` directory:

```bash
cd addicted.nuxt
pnpm exec playwright test
```

### Desktop verification

1. Navigate to the page under test.
2. Take a full-page screenshot to confirm layout.
3. Interact with elements (expand season panels, click download buttons) and
   screenshot again.
4. Check the browser console for hydration mismatch warnings — these indicate
   SSR / client rendering differences that must be fixed.

### Mobile verification

Because `@nuxtjs/device` detects mobile from the **server-side** `User-Agent`
header you must emulate mobile on the request, not just the viewport:

1. Open a **new browser tab** so that `context.addInitScript` does not bleed
   into other tests.
2. Override the User-Agent header and viewport:

   ```js
   const context = page.context();
   const mobileUA =
     "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) " +
     "AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1";
   await context.setExtraHTTPHeaders({ "User-Agent": mobileUA });
   await context.addInitScript(() => {
     Object.defineProperty(navigator, "userAgent", {
       get: () =>
         "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) " +
         "AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1",
     });
   });
   await page.setViewportSize({ width: 390, height: 844 });
   ```

3. Navigate to the page, take screenshots, and interact with elements.
4. Check the console for hydration mismatch warnings.

---

## 6 — Stopping the stack

```bash
docker compose -f docker-compose.dev.yml down
```

To also remove the built images (forces a full rebuild next time):

```bash
docker compose -f docker-compose.dev.yml down --rmi local
```

---

## 7 — Rebuilding a single service

```bash
# Rebuild and restart only the mock API
docker compose -f docker-compose.dev.yml up --build mock-api

# Rebuild and restart only the frontend
docker compose -f docker-compose.dev.yml up --build frontend
```

---

## 8 — Troubleshooting

| Symptom                                            | Likely cause                                                                                | Fix                                                                                      |
| -------------------------------------------------- | ------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| `frontend` fails to fetch data in Docker Compose   | `NUXT_PUBLIC_API_SERVER_URL` must use the Docker service name (`mock-api`), not `localhost` | Verify the env var in `docker-compose.dev.yml`                                           |
| `frontend` fails to fetch data in native local dev | `APP_SERVER_PATH` must point at the running mock API, usually `http://localhost:8080`       | Start the mock server and export the local `APP_*` variables before `pnpm dev`           |
| Homepage throws `Invalid URL` during SSR           | `APP_URL` is unset, so SEO URL generation has no valid base URL                             | Set `APP_URL=http://localhost:3000` in local/dev environments                            |
| Port already in use                                | Another process is using 3000 or 8080                                                       | Stop the conflicting process or change the port mapping in `docker-compose.dev.yml`      |
| Mock API returns unexpected data                   | Code changes in `mock-server/main.go` not picked up                                         | Re-run with `--build`                                                                    |
| Frontend shows old UI                              | Nuxt build cache                                                                            | Re-run with `--build`                                                                    |
| `go build` fails inside Docker                     | Go version mismatch                                                                         | Check `mock-server/go.mod` and the `FROM golang:` line in `mock-server/Dockerfile` match |

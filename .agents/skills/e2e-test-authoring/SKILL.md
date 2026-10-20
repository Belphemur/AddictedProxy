---
name: e2e-test-authoring
description: How to write and run Playwright end-to-end tests for the AddictedProxy Nuxt frontend against the Go mock server. Use this skill whenever asked to add, update, or debug an e2e test.
---

The AddictedProxy frontend has a Playwright e2e test suite in
`addicted.nuxt/e2e/`, configured by `addicted.nuxt/playwright.config.ts`.
Tests always run against the **mock API server** (`mock-server/`), never
against a real .NET backend — see the `mock-environment-setup` skill for how
the mock stack works and what shows/data it returns.

CI runs this suite automatically for any PR touching `addicted.nuxt/**` or
`mock-server/**` via `.github/workflows/reusable-nuxt-e2e.yml`, and it is one
of the required checks aggregated by the `CI Gate` job in
`.github/workflows/ci-cd.yml`.

---

## 1 — Where things live

| Concern                | Location                                             |
| ----------------------- | ----------------------------------------------------- |
| Test files              | `addicted.nuxt/e2e/*.spec.ts`                         |
| Playwright config       | `addicted.nuxt/playwright.config.ts`                  |
| Mock API server         | `mock-server/` (Go, see its `README.md`)              |
| CI job                  | `.github/workflows/reusable-nuxt-e2e.yml`             |

Test files are matched by `testDir: "./e2e"` in the Playwright config, so any
`*.spec.ts` file added under `addicted.nuxt/e2e/` is picked up automatically —
no registration step needed.

---

## 2 — Adding a new e2e test

1. Create a new file `addicted.nuxt/e2e/<feature>.spec.ts`.
2. Import from `@playwright/test`:

   ```ts
   import { expect, test } from "@playwright/test";
   ```

3. Group related tests with `test.describe(...)` and use `page.goto("/…")` —
   the `baseURL` (mock-backed app) is already configured, so use relative
   paths.
4. Prefer resilient, accessibility-based locators over CSS classes:
   - `page.getByRole("heading", { name: "..." })`
   - `page.getByRole("textbox", { name: "..." })` (do **not** use `getByLabel`
     on Vuetify `clearable` fields — the clear icon shares the same
     accessible label and triggers "strict mode" locator errors; use
     `getByRole("textbox", ...)` instead)
   - `page.getByRole("link", { name: /.../ })` instead of `getByText(...)` when
     the same text may also appear in an off-screen/placeholder element (e.g.
     `LazyOptimizedPicture`'s SVG placeholder duplicates the card title).
5. When simulating typing into a Vuetify `v-text-field` bound with
   `@input`/`v-model`, use `locator.click()` followed by
   `locator.pressSequentially(text, { delay: 30 })` rather than
   `locator.fill(...)`. Vuetify's internal input handling can behave
   inconsistently with `fill()`'s single DOM `input` event across browser
   engines (notably WebKit), causing flaky tests.
6. Reference known mock data (show IDs, names, season counts) from
   `mock-server/README.md` instead of hard-coding assumptions — the mock
   catalog is the source of truth for what the UI should render.
7. Keep tests independent — each test should `page.goto(...)` itself; don't
   rely on state left over from a previous test.

---

## 3 — Running tests locally

Start the mock server and a **production build** of the Nuxt app (not
`pnpm dev` — see §4 for why), then run Playwright:

```bash
# Terminal 1 — mock API
cd mock-server
go run . -port 8080

# Terminal 2 — build & start the Nuxt app pointed at the mock API
cd addicted.nuxt
NUXT_PRESET=node-server APP_URL=http://localhost:3000 \
  APP_API_PATH=http://localhost:8080 APP_SERVER_PATH=http://localhost:8080 \
  SENTRY_ENABLE=false pnpm build
cd .output/server && rm -rf node_modules && pnpm install && pnpm add ws && cd ../..
NUXT_HOST=0.0.0.0 NUXT_PORT=3000 APP_URL=http://localhost:3000 \
  APP_API_PATH=http://localhost:8080 APP_SERVER_PATH=http://localhost:8080 \
  SENTRY_ENABLE=false node .output/server/index.mjs

# Terminal 3 — run the tests
cd addicted.nuxt
pnpm exec playwright install --with-deps chromium webkit   # first time only
pnpm test:e2e
```

Useful variants:

```bash
pnpm test:e2e:ui                 # Playwright's interactive UI mode
pnpm exec playwright test e2e/home.spec.ts --project=chromium
pnpm exec playwright show-trace test-results/<test>/trace.zip   # inspect a failure
```

`E2E_BASE_URL` (defaults to `http://localhost:3000`) overrides the base URL
used by `playwright.config.ts`, matching how CI points tests at the
already-running app.

---

## 4 — Why a production build, not `pnpm dev`

Do **not** run e2e tests against `pnpm dev`. Nuxt's dev server file-watcher
watches the whole `addicted.nuxt/` directory, including
`test-results/`, `playwright-report/`, and other Playwright output written
*during* the test run. Those writes are picked up as source changes and
trigger a full HMR client reload mid-test, silently resetting component state
(e.g. clearing a search input a few hundred milliseconds after typing) and
causing hard-to-diagnose flakiness. Always build (`NUXT_PRESET=node-server
pnpm build`) and run the compiled output (`node .output/server/index.mjs`)
for e2e testing, exactly as `reusable-nuxt-e2e.yml` and the production
`Dockerfile` do.

The `.output/server` runtime is missing the optional `ws` dependency that
`@microsoft/signalr` needs server-side; without `pnpm add ws` in
`.output/server`, any page using the refresh SignalR hub (e.g. show details)
will 500 with `Cannot find module 'ws'`.

---

## 5 — Troubleshooting

| Symptom                                                    | Likely cause                                                                                   | Fix                                                                                          |
| ------------------------------------------------------------ | -------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| Input value clears itself ~0.5–1s after typing               | Testing against `pnpm dev`; file-watcher triggered an HMR reload from Playwright's own artifacts | Build + run production output instead (see §4)                                              |
| `strict mode violation` resolving 2 elements for a label     | `getByLabel` matched both the input and its Vuetify clear icon                                   | Use `getByRole("textbox", { name: ... })`                                                    |
| Show details page returns HTTP 500 "Cannot find module 'ws'" | `.output/server/node_modules` is missing the `ws` package                                        | Run `cd .output/server && rm -rf node_modules && pnpm install && pnpm add ws`                |
| Click doesn't navigate / lands on the wrong element          | `getByText(...)` matched a duplicate node (e.g. image placeholder text mirrors a card title)     | Scope the locator to a container class, or use a more specific role query                    |
| Mock API returns unexpected data                             | `mock-server/data/*.json` changed, or the wrong port is used                                     | Confirm `APP_API_PATH`/`APP_SERVER_PATH` point at the running mock server                     |

---

## 6 — CI wiring

`.github/workflows/reusable-nuxt-e2e.yml` mirrors §3 exactly: it starts the
Go mock server, builds the Nuxt app for production, patches in the `ws`
dependency, starts the built server, then runs `pnpm exec playwright test`.
It is invoked from `.github/workflows/ci-cd.yml` as the `nuxt-e2e` job
whenever `addicted.nuxt/**` or `mock-server/**` changes, and its result feeds
into the `CI Gate` job that blocks merges on failure.

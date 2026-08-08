import { defineConfig, devices } from "@playwright/test";

/**
 * Playwright configuration for AddictedProxy frontend e2e tests.
 *
 * Tests run against the Nuxt app wired to the Go mock server
 * (see mock-server/README.md and .github/skills/mock-environment-setup).
 *
 * Local usage:
 *   1. Start the mock API:      cd mock-server && go run .
 *   2. Start the Nuxt app:      cd addicted.nuxt && APP_URL=http://localhost:3000 \
 *        APP_API_PATH=http://localhost:8080 APP_SERVER_PATH=http://localhost:8080 \
 *        SENTRY_ENABLE=false pnpm dev
 *   3. Run the tests:           cd addicted.nuxt && pnpm test:e2e
 *
 * In CI, the `E2E_BASE_URL` env var points at the already-running app instead
 * of relying on Playwright's `webServer` to start one (see
 * .github/workflows/reusable-nuxt-e2e.yml).
 */
export default defineConfig({
  testDir: "./e2e",
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 2 : undefined,
  reporter: process.env.CI ? [["html", { open: "never" }], ["github"]] : "list",
  use: {
    baseURL: process.env.E2E_BASE_URL ?? "http://localhost:3000",
    trace: "on-first-retry",
    screenshot: "only-on-failure",
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
    {
      name: "mobile-safari",
      use: { ...devices["iPhone 13"] },
    },
  ],
});

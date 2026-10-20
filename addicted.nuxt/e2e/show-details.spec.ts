import { expect, test } from "@playwright/test";

/**
 * Show details e2e tests against the mock server's fixed catalog.
 * See mock-server/README.md and .agents/skills/mock-environment-setup for
 * the full list of mock shows and expected behaviour.
 */
test.describe("Show details", () => {
  test("renders season packs and episodes for a regular show", async ({ page }) => {
    await page.goto("/shows/6TTo3qLDnBJgjloPOYLLKT/breaking-bad");

    await expect(page.getByRole("heading", { name: /Season \d+/ })).toBeVisible();
    await expect(page.getByRole("heading", { name: "Season Packs" })).toBeVisible();
    await expect(page.getByRole("heading", { name: "Episodes" })).toBeVisible();
  });

  test("hides the Episodes heading for a season-pack-only show", async ({ page }) => {
    await page.goto("/shows/6TTo3qMQerhZH6zvNexUjU/only-season-pack");

    await expect(page.getByRole("heading", { name: /Season \d+/ })).toBeVisible();
    await expect(page.getByRole("heading", { name: "Season Packs" })).toBeVisible();
    await expect(page.getByRole("heading", { name: "Episodes" })).toBeHidden();
  });
});

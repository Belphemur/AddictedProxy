import { expect, test } from "@playwright/test";

/**
 * Homepage e2e tests, run against the Nuxt app backed by the Go mock server.
 * See mock-server/README.md for the fixed set of shows returned.
 */
test.describe("Homepage", () => {
  test("renders the search box and trending shows from the mock server", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByRole("heading", { name: "Gestdown" })).toBeVisible();
    await expect(page.getByRole("textbox", { name: "Name of the show" })).toBeVisible();

    await expect(page.getByRole("heading", { name: "Trending" })).toBeVisible();

    // The mock server always returns these shows, see mock-server/README.md.
    await expect(page.getByRole("link", { name: /Breaking Bad/ })).toBeVisible();
    await expect(page.getByRole("link", { name: /Game of Thrones/ })).toBeVisible();
  });

  test("has no hydration mismatch warnings in the console", async ({ page }) => {
    const consoleErrors: string[] = [];
    page.on("console", (message) => {
      if (message.type() === "error" && /hydration/i.test(message.text())) {
        consoleErrors.push(message.text());
      }
    });

    await page.goto("/");
    await expect(page.getByRole("heading", { name: "Trending" })).toBeVisible();

    expect(consoleErrors).toEqual([]);
  });
});

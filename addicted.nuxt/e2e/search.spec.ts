import { expect, test } from "@playwright/test";

/**
 * Search e2e tests, exercising the show autocomplete against the mock
 * server's `/shows/search/{query}` endpoint.
 */
test.describe("Show search", () => {
  test("finds and navigates to a show from the mock server", async ({ page }) => {
    await page.goto("/");

    const searchInput = page.getByRole("textbox", { name: "Name of the show" });
    await searchInput.click();
    await searchInput.pressSequentially("Breaking", { delay: 30 });

    const result = page.locator(".results-list").getByText("Breaking Bad", { exact: false }).first();
    await expect(result).toBeVisible();
    await result.click();

    await expect(page).toHaveURL(/\/shows\/a1b2c3d4-0001-0001-0001-000000000001\//);
    await expect(page.getByRole("heading", { name: /Season \d+/ })).toBeVisible();
  });

  test("shows no results for a query that does not match any mock show", async ({ page }) => {
    await page.goto("/");

    const searchInput = page.getByRole("textbox", { name: "Name of the show" });
    await searchInput.click();
    const searchResponse = page.waitForResponse((response) => {
      const url = decodeURIComponent(response.url()).toLowerCase();
      return response.ok() && url.endsWith("/shows/search/zzzznonexistentshow");
    });
    await searchInput.pressSequentially("Zzzznonexistentshow", { delay: 30 });
    await searchResponse;

    await expect(page).toHaveURL("/");
  });
});

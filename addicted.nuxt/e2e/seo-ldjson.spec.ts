import { expect, test } from "@playwright/test";

/**
 * LLM/SEO structured-data e2e tests against the mock server.
 * See docs/2026-10-21-llm-discoverability.md and the mock-environment-setup
 * skill for the fixed set of mock shows.
 *
 * The pages are fetched as raw SSR HTML (`page.request.get`) because that is
 * exactly what crawlers and LLM agents consume — no JavaScript execution.
 */
type JsonLdNode = { "@type"?: string | string[]; [key: string]: unknown };

function extractJsonLdNodes(html: string): JsonLdNode[] {
  const nodes: JsonLdNode[] = [];
  const scripts = [
    ...html.matchAll(/<script[^>]*type="application\/ld\+json"[^>]*>([\s\S]*?)<\/script>/g),
  ];
  for (const [, body] of scripts) {
    const parsed: unknown = JSON.parse(body);
    const roots = Array.isArray(parsed) ? parsed : [parsed];
    for (const root of roots) {
      if (!root || typeof root !== "object") continue;
      const node = root as JsonLdNode;
      nodes.push(node);
      // Some producers emit a top-level @graph instead of a flat list.
      if (Array.isArray(node["@graph"])) {
        nodes.push(...(node["@graph"] as JsonLdNode[]));
      }
    }
  }
  return nodes;
}

test.describe("SEO structured data (JSON-LD)", () => {
  test("show page SSR HTML exposes a TVSeries node with the show name", async ({ request }) => {
    // Breaking Bad, from the mock server's fixed catalog.
    const response = await request.get("/shows/4v7SXznCWD5QQLn9P9mSaf/breaking-bad");
    expect(response.status()).toBe(200);
    const html = await response.text();

    const nodes = extractJsonLdNodes(html);
    const tvSeries = nodes.find((node) => node["@type"] === "TVSeries");
    expect(tvSeries).toBeTruthy();
    expect((tvSeries as { name?: string }).name).toBe("Breaking Bad");
  });

  test("show page JSON-LD never contains a download URI or downloadUri key", async ({ request }) => {
    const response = await request.get("/shows/4v7SXznCWD5QQLn9P9mSaf/breaking-bad");
    const html = await response.text();

    const nodes = extractJsonLdNodes(html);
    expect(nodes.length).toBeGreaterThan(0);
    // Download-wall invariant: metadata out, no download links, ever.
    const serialized = JSON.stringify(nodes);
    expect(serialized).not.toContain("/subtitles/download/");
    expect(serialized).not.toContain("downloadUri");
    expect(serialized).not.toContain('"sp_');
  });

  test("show page JSON-LD lists seasons and rendered episodes without subtitles", async ({ request }) => {
    const response = await request.get("/shows/4v7SXznCWD5QQLn9P9mSaf/breaking-bad");
    const html = await response.text();

    const nodes = extractJsonLdNodes(html);
    const tvSeries = nodes.find((node) => node["@type"] === "TVSeries") as {
      containsSeason?: Array<{ "@type"?: string; seasonNumber?: number }>;
    };
    expect(tvSeries.containsSeason?.length).toBeGreaterThan(0);
    expect(tvSeries.containsSeason?.every((season) => season["@type"] === "TVSeason")).toBe(true);

    const episodes = nodes.filter((node) => node["@type"] === "TVEpisode");
    expect(episodes.length).toBeGreaterThan(0);
    expect(
      episodes.every(
        (episode) =>
          (episode as { name?: string }).name && (episode as { url?: string }).url?.includes("/shows/"),
      ),
    ).toBe(true);
  });

  test("home page exposes a WebSite node", async ({ request }) => {
    const response = await request.get("/");
    expect(response.status()).toBe(200);
    const html = await response.text();

    const nodes = extractJsonLdNodes(html);
    expect(nodes.some((node) => node["@type"] === "WebSite")).toBeTruthy();
  });

  test("/llms.txt is served as non-empty text/plain without download URIs", async ({ request }) => {
    // The Go mock server does not implement the backend sitemap routes, so
    // locally /llms.txt cannot list show pages (design open item #5) — assert
    // the served shape; catalog-content verification is post-deploy.
    const response = await request.get("/llms.txt");
    expect(response.status()).toBe(200);
    expect(response.headers()["content-type"]).toContain("text/plain");

    const body = await response.text();
    expect(body.trim().length).toBeGreaterThan(0);
    // Download-wall invariant holds for every SEO surface.
    expect(body).not.toContain("/subtitles/download/");
  });

  test("/llms-full.txt is served without download URIs", async ({ request }) => {
    const response = await request.get("/llms-full.txt");
    expect(response.status()).toBe(200);

    const body = await response.text();
    expect(body.trim().length).toBeGreaterThan(0);
    expect(body).not.toContain("/subtitles/download/");
  });
});
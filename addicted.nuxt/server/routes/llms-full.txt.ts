/**
 * Nitro route for `/llms-full.txt` — proxies to the backend `SitemapController`
 * LlmFullCatalog action. The backend builds and caches the catalog (design:
 * docs/2026-10-21-llm-discoverability.md, backend-llms direction).
 *
 * Shares the single proxy config in server/sitemap/sitemap-proxy.ts (DRY);
 * see server/routes/sitemap.xml.ts for why this must be a mounted Nitro route.
 */
import sitemapProxy from "../sitemap/sitemap-proxy";

export default sitemapProxy;

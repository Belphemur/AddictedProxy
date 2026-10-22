/**
 * Nitro catch-all route for `/sitemap/**` — proxies the backend's chunked
 * sitemaps (`/sitemap/shows/{page}`, `/sitemap/media/{page}`), which
 * nuxt-ai-ready follows when crawling the sitemap index for llms.txt.
 *
 * Shares the single proxy config in server/sitemap/sitemap-proxy.ts (DRY);
 * see server/routes/sitemap.xml.ts for why this must be a mounted Nitro route.
 */
import sitemapProxy from "../../sitemap/sitemap-proxy";

export default sitemapProxy;

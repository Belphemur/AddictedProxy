/**
 * Nitro route for `/sitemap.xml` — proxies to the backend `SitemapController`.
 *
 * One proxy config, shared with `/sitemap/**` (server/routes/sitemap/[...slug].ts):
 * the handler itself lives in server/sitemap/sitemap-proxy.ts (DRY).
 *
 * This mount is what makes nuxt-ai-ready's llms.txt generation work: it resolves
 * the catalog through an INTERNAL `event.fetch("/sitemap.xml")` that hits this
 * server's own router — it never traverses Traefik, so without this route the
 * fetch 404s and `/llms.txt` lists nothing (review finding, PR #3237).
 */
import sitemapProxy from "../sitemap/sitemap-proxy";

export default sitemapProxy;

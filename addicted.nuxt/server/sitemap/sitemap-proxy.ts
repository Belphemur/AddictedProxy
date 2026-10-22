import { createProxyEventHandler } from "h3-proxy"

// Mounted by server/routes/sitemap.xml.ts, server/routes/sitemap/[...slug].ts,
// server/routes/llms.txt.ts and server/routes/llms-full.txt.ts
// (one proxy config for all mounts — DRY).
//
// The handler is created lazily on first request: the backend target comes
// from useRuntimeConfig(event), and at module load time there is no request
// context — the runtime config is not populated yet, so a module-scope target
// resolves to undefined (this exact bug shipped unmounted for two years and
// only surfaced once the route mount made the handler run).
//
// pathFilter semantics: h3-proxy string filters are SUBSTRING matches against
// the request path, and a pathFilter array mixing plain strings with globs
// throws ERR_CONTEXT_MATCHER_INVALID_ARRAY — so use all-string entries:
// "/sitemap.xml" covers the index, "/sitemap/" the chunked child sitemaps,
// and "/llms" the llms.txt catalogs (exact and full).
let proxy: ReturnType<typeof createProxyEventHandler> | undefined

export default defineEventHandler((event) => {
    proxy ??= createProxyEventHandler({
        target: useRuntimeConfig(event).public.api.serverUrl,
        changeOrigin: false,
        pathRewrite: {},
        pathFilter: ["/sitemap.xml", "/sitemap/", "/llms.txt", "/llms-full.txt"],
    })
    return proxy(event)
})

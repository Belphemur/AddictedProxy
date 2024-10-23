import { createProxyEventHandler } from "h3-proxy"

export default defineEventHandler(
    createProxyEventHandler({
        target: useRuntimeConfig().public.api.serverUrl,
        changeOrigin: false,
        pathRewrite: {},
        pathFilter: ["/sitemap/**"],
    }),
)
// https://nuxt.com/docs/api/configuration/nuxt-config

import { sentryVitePlugin } from "@sentry/vite-plugin";

const manualChunk = [
  "@sentry/vue",
  "@sentry/tracing",
  "@sentry/browser",
  "lodash-es",
];

export default defineNuxtConfig({
  experimental: {
    sharedPrerenderData: true,
    // Reuse Vite's file watcher instead of spinning up a second one (less
    // memory, fewer file handles in dev). Default in Nuxt 5.
    watcher: "builder",
    // Forward the destination page's preload/modulepreload hints as
    // rel="prefetch" when a link is prefetched, so navigations feel instant.
    prefetchPreloadTags: true,
  },
  routeRules: {
    "/": { swr: 120 },
    "/api": { prerender: true },
    "/privacy": { prerender: true },
    "/dmca": { swr: 3600 },
    // Crawler-facing surfaces: show pages render full SSR HTML (with JSON-LD)
    // per crawler hit — cache for an hour instead of re-rendering every request.
    "/shows/**": { swr: 3600 },
    "/llms.txt": { swr: 3600 },
  },
  app: {
    head: {
      style: [
        {
          // Establish CSS layer order for Vuetify 4 — must appear before any Vuetify styles
          innerHTML:
            "@layer base, vuetify-core, vuetify-components, vuetify-overrides, vuetify-utilities, vuetify-final, overrides;",
          tagPriority: -100,
        },
      ],
    },
  },
  scripts: {
    assets: {
      fallbackOnSrcOnBundleFail: true,
    },
    registry: {
      googleAdsense: {
        client: "ca-pub-7284443005140816",
        trigger: "onNuxtReady",
      },
      cloudflareWebAnalytics: process.env.APP_CLOUDFLARE_ANALYTIC_TOKEN
        ? {
            token: process.env.APP_CLOUDFLARE_ANALYTIC_TOKEN,
            trigger: "onNuxtReady",
          }
        : false,
    },
  },
  build: {
    transpile: ["picomatch"],
  },

  devtools: { enabled: true },
  sourcemap: { server: true, client: true },

  // @ts-ignore
  css: ["vuetify/styles"],

  vite: {
    // @ts-ignore
    // curently this will lead to a type error, but hopefully will be fixed soon #justBetaThings
    ssr: {
      noExternal: ["vuetify"], // add the vuetify vite plugin
    },
    vue: {
      script: {
        defineModel: true,
      },
    },
    build: {
      rollupOptions: {
        output: {
          manualChunks(id) {
            const separateModule = manualChunk.find((module) =>
              id.includes(module),
            );
            if (separateModule) return separateModule;
          },
        },
      },
    },
    plugins: [
      sentryVitePlugin({
        authToken: process.env.SENTRY_AUTH_TOKEN,
        org: process.env.SENTRY_ORG,
        project: process.env.SENTRY_PROJECT,
        telemetry: false,
        disable: process.env.NODE_ENV !== "production",
        release: {
          name: process.env.RELEASE_VERSION,
        },
        debug: true,
      }),
    ],
  },

  runtimeConfig: {
    public: {
      // Default to www.subvault.tv so prerendered/static pages bake a valid URL
      // instead of undefined when APP_URL isn't passed at build time. Override
      // via APP_URL (build) or NUXT_PUBLIC_URL (runtime) — see docs/domain-configurability-plan.md.
      url: process.env.APP_URL ?? process.env.NUXT_PUBLIC_URL ?? "https://www.subvault.tv",
      // Configurable brand name (defaults to the subvault.tv brand). Override at
      // build time (APP_SITENAME) or runtime (NUXT_PUBLIC_SITENAME). See
      // composables/useSiteBrand.ts and docs/domain-configurability-plan.md.
      siteName: process.env.APP_SITENAME ?? process.env.NUXT_PUBLIC_SITENAME ?? "Subvault",
      // Default to the subvault.tv docs host so the API page links to the
      // current documentation. Override at build time (APP_API_DOCS_URL) or
      // runtime (NUXT_PUBLIC_API_DOCS_URL). See docs/domain-configurability-plan.md.
      apiDocsUrl: process.env.APP_API_DOCS_URL ?? process.env.NUXT_PUBLIC_API_DOCS_URL ?? "https://docs.subvault.tv/",
      ads: {
        enabled: process.env.NUXT_PUBLIC_ADS_ENABLED !== "false",
      },
      api: {
        clientUrl: process.env.APP_API_PATH,
        serverUrl: process.env.APP_SERVER_PATH,
      },
      cloudflare: {
        analyticToken: process.env.APP_CLOUDFLARE_ANALYTIC_TOKEN,
      },
      matomo: {
        url: process.env.APP_MATOMO,
      },
      faro: {
        url: process.env.APP_FARO_URL,
        env: process.env.SENTRY_ENVIRONMENT ?? "local",
      },
      sentry: {
        config: {
          environment: process.env.SENTRY_ENVIRONMENT ?? "local",
          dsn: process.env.SENTRY_DSN,
          enabled:
            process.env.SENTRY_ENABLE !== undefined
              ? process.env.SENTRY_ENABLE == "true"
              : true,
        },
        serverConfig: {
          // Set sampling rate for profiling - this is relative to tracesSampleRate
          profilesSampleRate: parseFloat(
            process.env.SENTRY_TRACES_SAMPLE_RATE ?? "0.1",
          ),
        },
        clientConfig: {
          // This sets the sample rate to be 10%. You may want this to be 100% while
          // in development and sample at a lower rate in production
          replaysSessionSampleRate: parseFloat(
            process.env.SENTRY_REPLAY_SAMPLE_RATE ?? "0.1",
          ),
          // If the entire session is not sampled, use the below sample rate to sample
          // sessions when an error occurs.
          replaysOnErrorSampleRate: parseFloat(
            process.env.SENTRY_ERROR_REPLAY_SAMPLE_RATE ?? "1",
          ),
        },
      },
    },
  },

  googleFonts: {
    families: {
      Roboto: [100, 300, 400, 500, 700, 900],
    },
  },

  imports: {
    dirs: ["./stores"],
  },

  pinia: {
    storesDirs: ["./stores/**"],
  },
  vuetify: {
    vuetifyOptions: {
      icons: {
        defaultSet: "mdi-svg",
      },
      theme: {
        defaultTheme: "dark",
        themes: {
          dark: {
            colors: {
              primary: "#1867C0",
              secondary: "#5CBBF6",
            },
          },
          light: {
            colors: {
              primary: "#1867C0",
              secondary: "#5CBBF6",
            },
          },
        },
      },
    },
  },

  modules: [
    "@nuxtjs/google-fonts",
    "@pinia/nuxt",
    "@nuxtjs/device",
    "vuetify-nuxt-module",
    "pinia-plugin-persistedstate",
    "@nuxt/scripts",
    // LLM discoverability (design: docs/2026-10-21-llm-discoverability.md).
    // Individual modules, NOT the @nuxtjs/seo umbrella — its sitemap/robots
    // modules would duplicate backend-owned producers (DRY).
    "nuxt-site-config",
    "nuxt-schema-org",
    "nuxt-ai-ready",
  ],
  site: {
    // Same env fallback chain as runtimeConfig.public.url / .siteName — one
    // set of envs, one brand source (DRY; no new env vars).
    url:
      process.env.APP_URL ??
      process.env.NUXT_PUBLIC_URL ??
      "https://www.subvault.tv",
    name:
      process.env.APP_SITENAME ??
      process.env.NUXT_PUBLIC_SITENAME ??
      "Subvault",
    description:
      "Search and download subtitles from Addic7ed and SuperSubtitles",
    defaultLocale: "en-US",
  },
  schemaOrg: {
    // Identity resolves its name/description/url from the `site` config (same
    // env fallback chain) — no hardcoded brand (DRY).
    identity: { type: "Organization" },
  },

  compatibilityDate: "2026-02-27",
});

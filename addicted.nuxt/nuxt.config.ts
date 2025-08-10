// https://nuxt.com/docs/api/configuration/nuxt-config

import {sentryVitePlugin} from "@sentry/vite-plugin";
import tsconfigPaths from "vite-tsconfig-paths";

const manualChunk = ["@sentry/vue", "@sentry/tracing", "@sentry/browser", "@microsoft/signalr", "lodash-es", "@microsoft/signalr-protocol-msgpack"];

export default defineNuxtConfig({
    experimental: {
        sharedPrerenderData: true
    },
    build: {
        transpile: ['picomatch', 'ws']
    },

    devtools: {enabled: true},
    sourcemap: {server: true, client: true},

    // @ts-ignore
    css: ['vuetify/styles'],

    vite: {
        // @ts-ignore
        // curently this will lead to a type error, but hopefully will be fixed soon #justBetaThings
        ssr: {
            noExternal: ['vuetify'], // add the vuetify vite plugin
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
                        const separateModule = manualChunk.find(module => id.includes(module));
                        if (separateModule) return separateModule;
                    }
                }
            }
        },
        plugins: [
            sentryVitePlugin({
                authToken: process.env.SENTRY_AUTH_TOKEN,
                org: process.env.SENTRY_ORG,
                project: process.env.SENTRY_PROJECT,
                telemetry: false,
                disable: process.env.NODE_ENV !== 'production',
                release: {
                    name: process.env.RELEASE_VERSION,
                },
                debug: true,
            }),
            tsconfigPaths()
        ]
    },

    runtimeConfig: {
        public: {
            url: process.env.APP_URL,
            api: {
                clientUrl: process.env.APP_API_PATH,
                serverUrl: process.env.APP_SERVER_PATH,
            },
            cloudflare: {
                analyticToken: process.env.APP_CLOUDFLARE_ANALYTIC_TOKEN
            },
            matomo: {
                url: process.env.APP_MATOMO
            },
            faro: {
                url: process.env.APP_FARO_URL,
                env: process.env.SENTRY_ENVIRONMENT ?? "local"
            },
            sentry: {
                config: {
                    environment: process.env.SENTRY_ENVIRONMENT ?? "local",
                    dsn: process.env.SENTRY_DSN,
                    enabled: process.env.SENTRY_ENABLE !== undefined ? process.env.SENTRY_ENABLE == "true" : true,
                },
                serverConfig: {
                    // Set sampling rate for profiling - this is relative to tracesSampleRate
                    profilesSampleRate: parseFloat(process.env.SENTRY_TRACES_SAMPLE_RATE ?? '0.1'),
                },
                clientConfig: {
                    // This sets the sample rate to be 10%. You may want this to be 100% while
                    // in development and sample at a lower rate in production
                    replaysSessionSampleRate: parseFloat(process.env.SENTRY_REPLAY_SAMPLE_RATE ?? '0.1'),
                    // If the entire session is not sampled, use the below sample rate to sample
                    // sessions when an error occurs.
                    replaysOnErrorSampleRate: parseFloat(process.env.SENTRY_ERROR_REPLAY_SAMPLE_RATE ?? '1'),
                }
            }
        },
    },

    googleFonts: {
        families: {
            Roboto: [100, 300, 400, 500, 700, 900],
        }
    },

    imports: {
        dirs: ['./stores'],
    },

    pinia: {
        storesDirs: ['./stores/**'],
    },
    vuetify: {
        vuetifyOptions: {
            icons: {
                defaultSet: 'mdi-svg'
            },
            theme: {
                defaultTheme: 'dark',
                themes: {
                    light: {
                        colors: {
                            primary: '#1867C0',
                            secondary: '#5CBBF6',
                        },
                    },
                },
            },
        }
    },

    modules: [
      '@nuxtjs/google-fonts',
      '@pinia/nuxt',
      '@nuxtjs/device',
      "vuetify-nuxt-module",
      'pinia-plugin-persistedstate'
    ],

    compatibilityDate: '2025-03-08'
})
// https://nuxt.com/docs/api/configuration/nuxt-config

import vuetify from "vite-plugin-vuetify";
import {sentryVitePlugin} from "@sentry/vite-plugin";

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
    css: ['vuetify/styles', '@mdi/font/css/materialdesignicons.css'],

    vite: {
        // @ts-ignore
        // curently this will lead to a type error, but hopefully will be fixed soon #justBetaThings
        ssr: {
            noExternal: ['vuetify'], // add the vuetify vite plugin
        },
        vue: {
            script: {
                defineModel: true,
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

    image: {
        domains: ['image.tmdb.org'],
        format: ['webp', 'jpeg'],
        providers: {
            gestdown: {
                name: 'gestdown', // optional value to overrider provider name
                provider: '~/server/image/tmdb/gestdown.ts', // Path to custom provider
            }
        }
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
        autoImports: ['defineStore', 'acceptHMRUpdate'],
    },

    proxy: {
        options: [
            {
                target: process.env.APP_SERVER_PATH,
                pathFilter: ['/sitemap/**'],
                pathRewrite: {}
            }
        ]
    },

    modules: [
        '@nuxtjs/google-fonts',
        '@pinia/nuxt',
        '@pinia-plugin-persistedstate/nuxt',
        '@nuxtjs/device',
        'nuxt-lodash',
        '@nuxt/image',
        // @ts-ignore
        // this adds the vuetify vite plugin
        // also produces type errors in the current beta release
        async (options, nuxt) => {
            nuxt.hooks.hook('vite:extendConfig', (config: any) => config.plugins.push(
                vuetify({
                    autoImport: true
                }),
            ))
        }
    ],

    compatibilityDate: '2024-10-22'
})
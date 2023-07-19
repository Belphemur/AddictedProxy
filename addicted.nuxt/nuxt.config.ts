// https://nuxt.com/docs/api/configuration/nuxt-config

import vuetify from "vite-plugin-vuetify";
import {sentryVitePlugin} from "@sentry/vite-plugin";


export default defineNuxtConfig({
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
                disable: process.env.SENTRY_ENVIRONMENT !== 'production',
                release: {
                    name: process.env.RELEASE_VERSION,
                },
                debug: true,
            }),
        ]
    },
    runtimeConfig: {
        public: {
            api: {
                clientUrl: process.env.APP_API_PATH,
                serverUrl: process.env.APP_SERVER_PATH,
            },
            matomo: {
                url: process.env.APP_MATOMO
            },
            sentry: {
                config: {
                    environment: process.env.SENTRY_ENVIRONMENT,
                    dsn: process.env.SENTRY_DSN,
                    enabled: true,
                },
                serverConfig: {
                    // Set sampling rate for profiling - this is relative to tracesSampleRate
                    profilesSampleRate: 1.0,
                },
                clientIntegrations: {
                    Replay: {},
                },
                clientConfig: {
                    // This sets the sample rate to be 10%. You may want this to be 100% while
                    // in development and sample at a lower rate in production
                    replaysSessionSampleRate: 0.1,
                    // If the entire session is not sampled, use the below sample rate to sample
                    // sessions when an error occurs.
                    replaysOnErrorSampleRate: 1.0,
                }
            }
        },
        ipx: {
            maxAge: 60 * 60 * 24 * 365,
            cache: true,
            domains: ['image.tmdb.org'],
            cacheOptions: {
                ttl: 2 * 24 * 60 * 60 * 1000,
                max: 50,
                updateAgeOnGet: true
            }
        }
    },
    image: {
        domains: ['image.tmdb.org'],
        format: ['avif', 'webp'],
        provider: 'ipx',
        ipx :{
            baseURL: '/_transform'
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
    ]
})

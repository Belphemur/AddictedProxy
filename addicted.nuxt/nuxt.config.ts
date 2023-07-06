// https://nuxt.com/docs/api/configuration/nuxt-config

import vuetify from "vite-plugin-vuetify";

export default defineNuxtConfig({
    devtools: {enabled: true},
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
        }
    },
    runtimeConfig: {
        public: {
            api: {
                clientUrl: process.env.APP_API_PATH,
                serverUrl: process.env.APP_SERVER_PATH,
            },
            matomo: {
                url: process.env.APP_MATOMO
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

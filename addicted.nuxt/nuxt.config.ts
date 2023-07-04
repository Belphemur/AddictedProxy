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
            VITE_APP_API_PATH: process.env.VITE_APP_API_PATH,
            VITE_APP_MATOMO: process.env.VITE_APP_MATOMO,
        }
    },
    googleFonts: {
        families: {
            Roboto: [100, 300, 400, 500, 700, 900],
        }
    },
    modules: [
        '@nuxtjs/google-fonts',
        '@nuxtjs/device',
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

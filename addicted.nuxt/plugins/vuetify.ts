// plugins/vuetify.js
import {createVuetify} from 'vuetify'
import {VDataTable} from "vuetify/labs/components";

export default defineNuxtPlugin(nuxtApp => {
    const vuetify = createVuetify({
        components: {
            VDataTable
        },
        ssr: true,
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
    })

    nuxtApp.vueApp.use(vuetify)
})
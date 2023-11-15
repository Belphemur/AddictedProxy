// plugins/vuetify.js
import {createVuetify} from 'vuetify'
import {VDataTable, VSkeletonLoader} from "vuetify/components";

export default defineNuxtPlugin(nuxtApp => {
    const vuetify = createVuetify({
        components: {
            VDataTable,
            VSkeletonLoader
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
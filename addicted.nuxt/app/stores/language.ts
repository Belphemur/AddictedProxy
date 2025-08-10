import { defineStore, acceptHMRUpdate } from 'pinia'

export const useLanguage = defineStore('language', {
    state: () => ({
        lang: 'en',
    }),
    persist: true
})

if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(useLanguage, import.meta.hot))
}

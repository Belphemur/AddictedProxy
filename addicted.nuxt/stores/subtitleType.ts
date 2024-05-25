import type {SubtitleType} from "~/composables/dto/SubtitleType";

export const useSubtitleType = defineStore('subtitleType', {
    state: () => ({
        type: 'normal' as SubtitleType,
    }),
    persist: true
})

if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(useSubtitleType, import.meta.hot))
}

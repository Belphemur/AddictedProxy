import type {SubtitleType} from "~/composables/dto/SubtitleType";
import { defineStore, acceptHMRUpdate } from 'pinia'

export const useSubtitleType = defineStore('subtitleType', {
    state: () => ({
        type: 'regular' as SubtitleType,
    }),
    persist: true
})

if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(useSubtitleType, import.meta.hot))
}

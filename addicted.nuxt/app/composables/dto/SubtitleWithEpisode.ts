import type {EpisodeDto, SubtitleDto} from "~/composables/api/data-contracts";

export interface SubtitleWithEpisode {
    subtitle: SubtitleDto
    episode: EpisodeDto
    title: string
}
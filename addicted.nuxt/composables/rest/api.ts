import {Subtitles} from "~/composables/api/Subtitles";
import {Shows} from "~/composables/api/Shows";
import {Media} from "~/composables/api/Media";


let subtitles: Subtitles<any>;
let shows: Shows<any>;
let media: Media<any>;

export function useSubtitles() {
    if (subtitles) return subtitles;
    const config = useRuntimeConfig();

    return subtitles = new Subtitles({
        // @ts-ignore
        baseUrl: config.public.api.url,
    });
}
export function useMedia() {
    if (media) return media;
    const config = useRuntimeConfig();

    return media = new Media({
        // @ts-ignore
        baseUrl: config.public.api.url,
    });
}

export function useShows() {
    if (shows) return shows;
    const config = useRuntimeConfig();

    return shows = new Shows({
        // @ts-ignore
        baseUrl: config.public.api.url,
    });
}

import {Subtitles} from "~/composables/api/Subtitles";
import {Shows} from "~/composables/api/Shows";
import {Media} from "~/composables/api/Media";


let subtitles: Subtitles<any>;
let shows: Shows<any>;
let media: Media<any>;

export function getApiServerUrl(): string {
    const config = useRuntimeConfig();
    return process.client ? config.public.api.clientUrl : config.public.api.serverUrl;
}

export function useSubtitles() {
    if (subtitles) return subtitles;
    const config = useRuntimeConfig();

    return subtitles = new Subtitles({
        // @ts-ignore
        baseUrl: getApiServerUrl(),
    });
}

export function useMedia() {
    if (media) return media;
    const config = useRuntimeConfig();

    return media = new Media({
        // @ts-ignore
        baseUrl: getApiServerUrl(),
    });
}

export function useShows() {
    if (shows) return shows;
    const config = useRuntimeConfig();

    return shows = new Shows({
        // @ts-ignore
        baseUrl: getApiServerUrl(),
    });
}

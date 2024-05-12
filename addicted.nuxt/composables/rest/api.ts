import {Subtitles} from "~/composables/api/Subtitles";
import {Shows} from "~/composables/api/Shows";
import {Media} from "~/composables/api/Media";
import type {ApiConfig} from "~/composables/api/http-client";


let subtitles: Subtitles<any>;
let shows: Shows<any>;
let media: Media<any>;

export function getApiServerUrl(): string {
    const config = useRuntimeConfig();
    return typeof window === "undefined" ? config.public.api.serverUrl : config.public.api.clientUrl ;
}

function getApiConfig() {
    const apiConfig : ApiConfig = {
        // @ts-ignore
        baseUrl: getApiServerUrl(),
        customFetch: input => {
            console.log("fetching", input)
            if(input.credentials !== undefined)
               delete input.credentials;
            return fetch(input)
        },
    };
    return apiConfig;
}


export function useSubtitles() {
    if (subtitles) return subtitles;

    return subtitles = new Subtitles(getApiConfig());
}

export function useMedia() {
    if (media) return media;

    return media = new Media(getApiConfig());
}

export function useShows() {
    if (shows) return shows;

    return shows = new Shows(getApiConfig());
}

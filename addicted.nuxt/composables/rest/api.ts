import {Subtitles} from "~/composables/api/Subtitles";
import {Shows} from "~/composables/api/Shows";
import {Media} from "~/composables/api/Media";
import type {ApiConfig} from "~/composables/api/http-client";
import * as Sentry from '@sentry/vue'


let subtitles: Subtitles<any>;
let shows: Shows<any>;
let media: Media<any>;

export function getApiServerUrl(): string {
    const config = useRuntimeConfig();
    return typeof window === "undefined" ? config.public.api.serverUrl : config.public.api.clientUrl;
}

function getApiConfig() {
    const apiConfig: ApiConfig = {
        // @ts-ignore
        baseUrl: getApiServerUrl(),
        customFetch: (input, init) => {
            //Clean up fetch request for server request of worker
            //Server side
            console.log("fetching", input)
            if (input.credentials !== undefined)
                delete input.credentials;
            if (input.mode !== undefined)
                delete input.mode;
            //Client side
            const activeSpan = Sentry.getActiveSpan();
            const rootSpan = activeSpan ? Sentry.getRootSpan(activeSpan) : undefined;

// Create `sentry-trace` header
            const sentryTraceHeader = rootSpan ? Sentry.spanToTraceHeader(rootSpan) : undefined;

// Create `baggage` header
            const sentryBaggageHeader = rootSpan ? Sentry.spanToBaggageHeader(rootSpan) : "undefined";

            let addedHeader = {}
            if (sentryBaggageHeader !== undefined && sentryTraceHeader !== undefined) {
                addedHeader = {
                    baggage: sentryBaggageHeader,
                    'sentry-trace': sentryTraceHeader,
                }
            }
            init = {
                ...init,
                headers: {
                    ...init?.headers,
                    ...addedHeader
                },
            };

            return fetch(input, init)
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

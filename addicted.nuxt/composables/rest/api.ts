import {Subtitles} from "~/composables/api/Subtitles";
import {Shows} from "~/composables/api/Shows";
import {Media} from "~/composables/api/Media";
import type {ApiConfig} from "~/composables/api/http-client";
import * as Sentry from '@sentry/vue'
import fetchRetry from 'fetch-retry';

let subtitles: Subtitles<any>;
let shows: Shows<any>;
let media: Media<any>;

// Create a new fetch function with retry capabilities
const fetchWithRetry = fetchRetry(fetch, {
    retries: 5,
    retryDelay: function (attempt, error, response) {
        console.log("retrying", attempt, error, response);
        return Math.pow(2, attempt) * 100 + (Math.random() * 100);
    },
    retryOn: [429, 503, 504, 500]
});

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
            if (typeof window === "undefined") {
                console.log("fetching", input, init)
                if (init?.credentials !== undefined)
                    delete init.credentials;
                if (init?.mode !== undefined)
                    delete init.mode;
                if (init?.referrerPolicy !== undefined)
                    delete init.referrerPolicy
                //Client side
            } else {
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
            }
            return fetchWithRetry(input, init)
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

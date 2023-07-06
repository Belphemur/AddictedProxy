import {Api} from "~/composables/api/api";

let api: Api<any>;

export function useApi() {
    if (api) return api;
    const config = useRuntimeConfig();

    return api = new Api({
        // @ts-ignore
        baseUrl: config.public.api.url,
    });
}

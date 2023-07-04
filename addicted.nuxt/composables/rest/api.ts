import {Api} from "@/api/api";

let api: Api<any>;

export function useApi() {
    if (api) return api;
    const config = useRuntimeConfig();

    return api = new Api({
        baseUrl: config.public.VITE_APP_API_PATH,
    });
}

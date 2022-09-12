import {SearchRequest} from "./api";

async function handlePostRequest(request: Request, ctx: ExecutionContext) {
    const regex = /(?<show>.+)S(?<season>\d+)E(?<episode>\d+)/g;
    const body = await request.json<SearchRequest>();
    if (body.search == undefined) {
        return new Response("No search", {
            status: 400
        })
    }
    if (body.language == undefined) {
        return new Response("No language", {
            status: 400
        })
    }
    const result = regex.exec(body.search);
    if (result == null) {
        return new Response("Bad search", {
            status: 400
        })
    }

    const cache = caches.default;

    // @ts-ignore
    const {groups: {show, season, episode}} = result;

    const cacheUrl = new URL(request.url);
    cacheUrl.pathname = `/show/search/${show}`;

    const cacheKey = new Request(cacheUrl.toString(), {
        method: 'GET',
        headers: request.headers
    })

    const finalUrl = new URL("https://api.gestdown.info");
    finalUrl.pathname = `/subtitles/find/${body.language.trim()}/${show.trim()}/${season.trim()}/${episode.trim()}`;

    const cacheResponse = await cache.match(cacheKey);
    if (cacheResponse != undefined) {
        console.log("Used cached response", finalUrl, cacheResponse.status)
        return cacheResponse;
    }

    const response = await fetch(finalUrl.toString(), {
        cf: {
            cacheEverything: true,
            cacheTtlByStatus: {'200-299': 7200, '423': 30, '429': 0, '404': 43200, '500-599': 0},
        },
        method: 'GET',
        headers: request.headers
    })

    if (response.status == 404) {
        console.log("cache 404", finalUrl);
        await cache.put(cacheKey, new Response("Unknown Show", {
            status: 404,
            headers: new Headers({"Cache-Control": "public, max-age=43200"})
        }));
    }
    return response;
}


export interface Env {
    // Example binding to KV. Learn more at https://developers.cloudflare.com/workers/runtime-apis/kv/
    // MY_KV_NAMESPACE: KVNamespace;
    //
    // Example binding to Durable Object. Learn more at https://developers.cloudflare.com/workers/runtime-apis/durable-objects/
    // MY_DURABLE_OBJECT: DurableObjectNamespace;
    //
    // Example binding to R2. Learn more at https://developers.cloudflare.com/workers/runtime-apis/r2/
    // MY_BUCKET: R2Bucket;
}

export default {
    async fetch(
        request: Request,
        env: Env,
        ctx: ExecutionContext
    ): Promise<Response> {
        if (request.method.toUpperCase() === 'POST') {
            return await handlePostRequest(request, ctx);
        }
        return await fetch(request);
    },
};

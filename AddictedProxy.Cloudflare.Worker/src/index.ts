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

    // @ts-ignore
    const {groups: {show, season, episode}} = result;

    return await fetch(`https://api.gestdown.info/subtitles/find/${body.language}/${show.trim()}/${season.trim()}/${episode.trim()}`, {
        cf: {
            cacheEverything: true,
            cacheTtl: 7200
        },
        method: 'GET',
        headers: request.headers
    })
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

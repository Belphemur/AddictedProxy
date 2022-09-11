/**
 * Welcome to Cloudflare Workers! This is your first worker.
 *
 * - Run `wrangler dev src/index.ts` in your terminal to start a development server
 * - Open a browser tab at http://localhost:8787/ to see your worker in action
 * - Run `wrangler publish src/index.ts --name my-worker` to publish your worker
 *
 * Learn more at https://developers.cloudflare.com/workers/
 */

async function sha256(message: string) {
    // encode as UTF-8
    const msgBuffer = new TextEncoder().encode(message);

    // hash the message
    const hashBuffer = await crypto.subtle.digest('SHA-256', msgBuffer);

    // convert bytes to hex string
    return [...new Uint8Array(hashBuffer)].map(b => b.toString(16).padStart(2, '0')).join('');
}

async function handlePostRequest(request: Request, ctx: ExecutionContext) {
    const body = await request.clone().text();

    // Hash the request body to use it as a part of the cache key
    const hash = await sha256(body);
    const cacheUrl = new URL(request.url);

    // Store the URL in cache by prepending the body's hash
    cacheUrl.pathname = '/posts' + cacheUrl.pathname + hash;


    const cache = await caches.open("addicted");

    // Convert to a GET to be able to cache
    const cacheKey = new Request(cacheUrl.toString(), {
        headers: request.headers,
        method: 'GET',
    });
    // Find the cache key in the cache
    let response = await cache.match(cacheKey);

    // Otherwise, fetch response to POST request from origin
    if (!response) {
        console.log("Cache ", false, cacheUrl)
        response = await fetch(request);
        //response.headers.set("Cache-Control", "public, max-age=7200");
        ctx.waitUntil(cache.put(cacheKey, response.clone()));
    } else {
        console.log("Cache ", true, cacheUrl)
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
        try {
            if (request.method.toUpperCase() === 'POST') {
                return await handlePostRequest(request, ctx);
            }
            return await fetch(request);
        } catch (e: any) {
            return new Response('Error thrown ' + e.message)
        }
    },
};

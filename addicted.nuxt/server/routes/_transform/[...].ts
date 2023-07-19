import {withLeadingSlash} from "ufo";
import {createStorage} from "unstorage";
import lruCacheDriver from "unstorage/drivers/lru-cache";
import {createIPX, createIPXMiddleware} from "ipx";


const ipxConfig = useRuntimeConfig().ipx;
const finalConfig = {
    ...ipxConfig,
    cacheMetadataStore: createStorage({
        driver: lruCacheDriver({
            ...ipxConfig.cacheOptions,
        }),
    })
};
const ipx = createIPX(finalConfig);
const middleware = createIPXMiddleware(ipx);
export default defineEventHandler(async (ctx) => {

    ctx.node.req.url = withLeadingSlash(ctx.context.params!._)

    await middleware(ctx.node.req, ctx.node.res);
});
export default defineNuxtPlugin((app) => {
    const {
        // @ts-ignore
        matomo: {url},
    } = useRuntimeConfig().public

    if (!url)
        return

    // @ts-expect-error: `_mtm` is not defined
    const _mtm = window._mtm = window._mtm || []

    _mtm.push({'mtm.startTime': (new Date().getTime()), 'event': 'mtm.Start'})
    useHead({
        script: [
            {
                src: `${url}`,
                async: true,
            },
        ],
    })
    const router = useRouter();
    router.afterEach((to, from) => {
        _mtm.push({
            event: 'virtual_page_view',
            virtualPagePath: to.path,
            virtualPageTitle: to.name,
            virtualPageReferer: from.path,
        })
    });
})
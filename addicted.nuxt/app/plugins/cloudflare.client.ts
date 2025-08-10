export default defineNuxtPlugin((app) => {
    const {
        // @ts-ignore
        cloudflare: {analyticToken},
    } = useRuntimeConfig().public

    useHead({
        script: [
            {
                src: "https://static.cloudflareinsights.com/beacon.min.js",
                async: true,
                defer: true,
                'data-cf-beacon': `{"token": "${analyticToken}"}`
            },
        ]
    })
});
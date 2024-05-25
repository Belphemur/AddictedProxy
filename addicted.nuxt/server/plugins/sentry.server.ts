import * as Sentry from '@sentry/node'
import {nodeProfilingIntegration} from '@sentry/profiling-node'
import {H3Error} from 'h3'

export default defineNitroPlugin((nitroApp) => {
    const {public: {sentry}} = useRuntimeConfig()


    // Initialize Sentry
    Sentry.init({
        enabled: sentry.config.enabled,
        dsn: sentry.config.dsn,
        environment: sentry.config.environment,
        integrations: [nodeProfilingIntegration()],
        // Set tracesSampleRate to 1.0 to capture 100%
        // of transactions for performance monitoring.
        // We recommend adjusting this value in production
        tracesSampleRate: sentry.serverConfig.profilesSampleRate,
        profilesSampleRate: sentry.serverConfig.profilesSampleRate,
    })

    // Here comes the hooks

    // Inside the plugin, after initializing sentry
    nitroApp.hooks.hook('error', (error) => {
        // Do not handle 404s and 422s
        if (error instanceof H3Error) {
            if (error.statusCode === 404 || error.statusCode === 422) {
                return
            }
        }

        Sentry.captureException(error)
    })
    nitroApp.hooks.hookOnce('close', async () => {
        await Sentry.close(2000)
    })
})
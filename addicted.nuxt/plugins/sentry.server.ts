import {defineNuxtPlugin, useRuntimeConfig} from '#app'
import * as Sentry from '@sentry/nuxt'

export default defineNuxtPlugin((nuxtApp) => {
    const {public: {sentry, api}} = useRuntimeConfig()

    Sentry.init({
        enabled: sentry.config.enabled,
        debug: false,
        dsn: sentry.config.dsn,
        environment: sentry.config.environment,
        // Set tracesSampleRate to 1.0 to capture 100%
        // of transactions for performance monitoring.
        // We recommend adjusting this value in production
        tracesSampleRate: sentry.serverConfig.profilesSampleRate,

        // Capture Replay for 10% of all sessions,
        // plus for 100% of sessions with an error
        replaysSessionSampleRate: sentry.clientConfig.replaysSessionSampleRate,
        replaysOnErrorSampleRate: sentry.clientConfig.replaysOnErrorSampleRate,
        profilesSampleRate: sentry.serverConfig.profilesSampleRate,
        tracePropagationTargets: ['localhost', api.clientUrl],
    })

    return {
        provide: {
            sentrySetContext: Sentry.setContext,
            sentrySetUser: Sentry.setUser,
            sentrySetTag: Sentry.setTag,
            sentryAddBreadcrumb: Sentry.addBreadcrumb,
            sentryCaptureException: Sentry.captureException,
        },
    }
})
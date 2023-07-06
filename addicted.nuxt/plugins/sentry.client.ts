import {defineNuxtPlugin, useRuntimeConfig} from '#app'
import * as Sentry from '@sentry/vue'

export default defineNuxtPlugin((nuxtApp) => {
    const config = useRuntimeConfig()
    const router = useRouter();

    Sentry.init({
        enabled: config.public.sentry.config.enabled,
        app: nuxtApp.vueApp,
        autoSessionTracking: true,
        debug: config.public.sentry.config.environment !== 'production',
        dsn: config.public.sentry.config.dsn,
        environment: config.public.sentry.config.environment,
        integrations: [
            Sentry.browserTracingIntegration({router}),
            Sentry.replayIntegration(),
        ],
        trackComponents: true,
        hooks: ['activate', 'create', 'destroy', 'mount', 'update'],
        // Set tracesSampleRate to 1.0 to capture 100%
        // of transactions for performance monitoring.
        // We recommend adjusting this value in production
        tracesSampleRate: config.public.sentry.serverConfig.profilesSampleRate,

        // Capture Replay for 10% of all sessions,
        // plus for 100% of sessions with an error
        replaysSessionSampleRate: config.public.sentry.clientConfig.replaysSessionSampleRate,
        replaysOnErrorSampleRate: config.public.sentry.clientConfig.replaysOnErrorSampleRate,
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
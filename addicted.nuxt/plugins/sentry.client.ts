import {defineNuxtPlugin, useRuntimeConfig} from '#app'
import * as Sentry from '@sentry/vue'

export default defineNuxtPlugin((nuxtApp) => {
    const config = useRuntimeConfig()
    const router = useRouter();

    Sentry.init({
        enabled: config.public.sentry.config.enabled,
        app: nuxtApp.vueApp,
        autoSessionTracking: true,
        debug: false,
        dsn: config.public.sentry.config.dsn,
        environment: config.public.sentry.config.environment,
        integrations: [
            Sentry.browserTracingIntegration({router, enableInp: true, enableHTTPTimings: true}),
            Sentry.replayIntegration(),
            Sentry.browserProfilingIntegration(),
            Sentry.breadcrumbsIntegration()
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
        profilesSampleRate: config.public.sentry.serverConfig.profilesSampleRate
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
import {
    getWebInstrumentations,
    initializeFaro
} from '@grafana/faro-web-sdk';
import { TracingInstrumentation } from '@grafana/faro-web-tracing';

export default defineNuxtPlugin((nuxtApp) => {
    const {
        // @ts-ignore
        faro: {url, env},
    } = useRuntimeConfig().public

    if (!url || !env)
        return
    initializeFaro({
        url: url,
        app: {
            name: 'Gestdown',
            version: '1.0.0',
            environment: env,
        },
        instrumentations: [
            // Mandatory, overwriting the instrumentations array would cause the default instrumentations to be omitted
            ...getWebInstrumentations(),

            // Initialization of the tracing package.
            // This packages is optional because it increases the bundle size noticeably. Only add it if you want tracing data.
            new TracingInstrumentation(),
        ],
    });
})
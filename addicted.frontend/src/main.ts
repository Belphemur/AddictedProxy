import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

import "~/styles/index.scss";
import "uno.css";
import { BrowserTracing } from "@sentry/tracing";
import * as Sentry from "@sentry/vue";
import { Replay } from "@sentry/replay";

const app = createApp(App);

Sentry.init({
  app,
  // eslint-disable-next-line @typescript-eslint/ban-ts-comment
  // @ts-ignore
  dsn: import.meta.env.VITE_APP_SENTRY_DSN,
  integrations: [
    new BrowserTracing({
      routingInstrumentation: Sentry.vueRouterInstrumentation(router),
      // eslint-disable-next-line @typescript-eslint/ban-ts-comment
      // @ts-ignore
      tracingOrigins: [import.meta.env.VITE_APP_API_DOMAIN, /^\//],
    }),
    new Replay({
      // This sets the sample rate to be 10%. You may want this to be 100% while
      // in development and sample at a lower rate in production
      sessionSampleRate: 1.0,

      // If the entire session is not sampled, use the below sample rate to sample
      // sessions when an error occurs.
      errorSampleRate: 1.0,

      // Mask all text content with asterisks (*). Passes text
      // content through to `maskTextFn` before sending to server.
      //
      // Defaults to true, uncomment to change
       maskAllText: false,

      // Block all media elements (img, svg, video, object,
      // picture, embed, map, audio)
      //
      // Defaults to true, uncomment to change
      // blockAllMedia: true,
    }),
  ],
  // Set tracesSampleRate to 1.0 to capture 100%
  // of transactions for performance monitoring.
  // We recommend adjusting this value in production
  tracesSampleRate: 1.0,
});

app.use(router).mount("#app");

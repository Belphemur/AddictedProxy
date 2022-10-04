import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

import "~/styles/index.scss";
import "uno.css";
import { BrowserTracing } from "@sentry/tracing";
import * as Sentry from "@sentry/vue";

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
  ],
  // Set tracesSampleRate to 1.0 to capture 100%
  // of transactions for performance monitoring.
  // We recommend adjusting this value in production
  tracesSampleRate: 1.0,
});

app.use(router).mount("#app");

import { Api } from "~/api/api";
import * as Sentry from "@sentry/vue";

export const api = new Api({
  baseUrl: import.meta.env.VITE_APP_API_PATH,
  customFetch: async (input, init) => {
    const existingTransaction = Sentry.getCurrentHub()
      .getScope()
      ?.getTransaction();

    // Create a transaction if one does not exist in order to work around
    // https://github.com/getsentry/sentry-javascript/issues/3169
    // https://github.com/getsentry/sentry-javascript/issues/4072
    const transaction =
      existingTransaction ??
      Sentry.startTransaction({
        name: `API Request: ${init!.method} ${input}`,
      });

    Sentry.getCurrentHub().configureScope((scope) =>
      scope.setSpan(transaction)
    );
    init = {
      ...init,
      headers: {
        ...init?.headers,
        ...Sentry.getCurrentHub().traceHeaders(),
      },
    };
    try {
      return await fetch(input, init);
    } finally {
      transaction.finish();
    }
  },
});

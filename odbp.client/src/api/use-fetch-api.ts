import { createFetch } from "@vueuse/core";

export const useFetchApi = createFetch({
  options: {
    beforeFetch({ options }) {
      // const token = "token";

      options.headers = {
        ...options.headers,
        "content-type": "application/json",
        "is-api": "true"
        // Authorization: `Bearer ${token}`
      };

      return { options };
    },
    afterFetch(ctx) {
      // console.log(ctx);
      return ctx;
    },
    onFetchError(ctx) {
      // console.log(ctx);
      return ctx;
    }
  }
});

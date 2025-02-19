import { asyncComputed } from "@vueuse/core";
import { readonly, ref } from "vue";

export function useLoader<T>(fetcher: (signal: AbortSignal) => Promise<T> | undefined) {
  const error = ref(false);
  const loading = ref(true);

  const data = asyncComputed(
    (onCancel) => {
      const abortController = new AbortController();

      onCancel(() => {
        abortController.abort();
      });

      error.value = false;

      return fetcher(abortController.signal)?.catch(() => {
        if (!abortController.signal.aborted) {
          error.value = true;
        }
        return undefined;
      });
    },
    undefined,
    loading
  );
  return {
    error: readonly(error),
    loading: readonly(loading),
    data: readonly(data)
  };
}

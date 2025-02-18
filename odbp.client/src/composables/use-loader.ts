import { asyncComputed } from "@vueuse/core";
import { ref, watch } from "vue";

export function useLoader<T>(
  fetcher: (signal: AbortSignal) => Promise<T> | undefined,
  options?: { showSpinnerAfterMs?: number }
) {
  const showSpinner = ref(false);
  const error = ref(false);
  const loading = ref(true);
  let timeout: number | undefined;

  watch(loading, (l) => {
    if (timeout) {
      clearTimeout(timeout);
    }
    if (l) {
      timeout = setTimeout(() => {
        showSpinner.value = true;
      }, options?.showSpinnerAfterMs ?? 200);
    } else {
      showSpinner.value = false;
    }
  });

  const data = asyncComputed(
    (onCancel) => {
      const abortController = new AbortController();

      onCancel(() => {
        abortController.abort();
      });

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
    error,
    loading,
    showSpinner,
    data
  };
}

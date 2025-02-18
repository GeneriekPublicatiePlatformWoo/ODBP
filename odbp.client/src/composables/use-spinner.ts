import { readonly, ref, toRef, watch, type MaybeRefOrGetter } from "vue";

const DEFAULT_SHOW_SPINNER_AFTER_MS = 200;

export const useSpinner = (
  loading: MaybeRefOrGetter<boolean>,
  options?: { showSpinnerAfterMs: number }
) => {
  const loadingRef = toRef(loading);
  const showSpinner = ref(false);

  let timeout: ReturnType<typeof setTimeout> | undefined;

  watch(
    loadingRef,
    (l) => {
      if (timeout) {
        clearTimeout(timeout);
      }
      if (l) {
        timeout = setTimeout(() => {
          showSpinner.value = true;
        }, options?.showSpinnerAfterMs ?? DEFAULT_SHOW_SPINNER_AFTER_MS);
      } else {
        showSpinner.value = false;
      }
    },
    { immediate: true }
  );

  return readonly(showSpinner);
};

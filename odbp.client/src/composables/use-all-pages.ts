import type { PagedResult } from "@/api";
import { asyncComputed } from "@vueuse/core";
import { ref } from "vue";

const fetchPage = <T>(url: string, signal?: AbortSignal | undefined) =>
  fetch(url, { headers: { "is-api": "true" }, signal })
    .then((r) => (r.ok ? r : Promise.reject(r)))
    .then((r) => r.json() as Promise<PagedResult<T>>);

export const fetchAllPages = async <T>(
  url: string,
  signal?: AbortSignal | undefined
): Promise<T[]> => {
  const { results, next } = await fetchPage<T>(url, signal);
  if (next) {
    const { pathname, search } = new URL(next);

    return [...results, ...(await fetchAllPages<T>(pathname + search, signal))];
  }

  return results;
};

export const useAllPages = <T>(url: string) => {
  const error = ref(false);
  const loading = ref(true);
  const data = asyncComputed(
    (onCancel) => {
      const abortController = new AbortController();

      onCancel(() => abortController.abort());

      return fetchAllPages<T>(url, abortController.signal).catch(() => {
        error.value = true;
        return [] as T[];
      });
    },
    [],
    loading
  );
  return {
    error,
    loading,
    data
  };
};

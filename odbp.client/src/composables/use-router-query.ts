import { type WritableComputedRef, computed } from "vue";
import { useRouter } from "vue-router";

const first = <T>(v: T | T[]) => (Array.isArray(v) ? v[0] : v);

export function useRouterQuerySingle<T extends string | number | null>(
  key: string,
  transform: (v: string | null) => T
): WritableComputedRef<T> {
  const router = useRouter();
  return computed({
    get() {
      return transform(first(router.currentRoute.value.query[key]));
    },
    set(v) {
      const { path, query } = router.currentRoute.value;
      router.push({
        path,
        query: {
          ...query,
          [key]: v?.toString()
        }
      });
    }
  });
}

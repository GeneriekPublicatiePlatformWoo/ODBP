import { type WritableComputedRef, computed, reactive, watch, watchEffect } from "vue";
import { useRouter } from "vue-router";

const first = <T>(v: T | T[]) => (Array.isArray(v) ? v[0] : v);

type QueryValue = string | null;

type GetSet<O> = {
  get: (v: QueryValue) => O;
  set: (v: O) => QueryValue;
};

type Get<O extends QueryValue> = (v: QueryValue) => O;

type InputRecord = Record<string, Get<QueryValue> | GetSet<unknown>>;

type GetOutput<T> = T extends GetSet<infer O> ? O : T extends Get<infer O> ? O : never;

type GetOutputRecord<T extends InputRecord> = {
  [K in keyof T]: T[K] extends GetOutput<infer O> ? O : never;
};

export function useSingleQueryValues<T extends InputRecord>(dict: T): GetOutputRecord<T> {
  const router = useRouter();
  const keys = Object.keys(dict) as Array<keyof T & string>;

  const obj = reactive(Object.fromEntries(keys.map((k) => [k, null]))) as GetOutputRecord<T>;

  watch(
    router.currentRoute,
    (route) => {
      Object.assign(
        obj,
        Object.fromEntries(
          keys.map((key) => {
            const transformer = dict[key] as Get<QueryValue> | GetSet<any>;
            const routeValue = first(route.query[key]);
            const transformerFunction =
              typeof transformer === "function" ? transformer : transformer.get;
            const mappedValue = transformerFunction(routeValue);
            return [key, mappedValue];
          })
        )
      );
    },
    { immediate: true }
  );

  watch(
    () => Object.entries(obj),
    (entries) =>
      router.push({
        path: router.currentRoute.value.path,
        query: {
          ...router.currentRoute.value.query,
          ...Object.fromEntries(
            entries.map(([key, value]) => {
              const transformer = dict[key] as Get<QueryValue> | GetSet<any>;
              const mappedValue =
                typeof transformer === "function" ? value : transformer.set(value);
              return [key, mappedValue];
            })
          )
        }
      })
  );

  return obj;
}

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

import { reactive, watch } from "vue";
import { useRouter, type LocationQueryValue } from "vue-router";

type SingleOrMultiple = LocationQueryValue | LocationQueryValue[];

export type GetSet<O> = {
  get: (v: LocationQueryValue[]) => O;
  set: (v: O) => SingleOrMultiple;
};

type Get<O extends SingleOrMultiple> = (v: LocationQueryValue[]) => O;

type InputRecord = Record<string, Get<SingleOrMultiple> | GetSet<any>>;

type GetOutput<T> = T extends GetSet<infer O> ? O : T extends Get<infer O> ? O : never;

type GetOutputRecord<T extends InputRecord> = {
  [K in keyof T]: T[K] extends GetOutput<infer O> ? O : never;
};

export function useQueryValues<T extends InputRecord>(dict: T): GetOutputRecord<T> {
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
            const transformer = dict[key] as Get<SingleOrMultiple> | GetSet<any>;
            const routeValue = route.query[key];
            const transformerFunction =
              typeof transformer === "function" ? transformer : transformer.get;
            const routeValueArr = Array.isArray(routeValue) ? routeValue : [routeValue];
            const mappedValue = transformerFunction(routeValueArr);
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
        query: Object.fromEntries(
          entries.map(([key, value]) => {
            const transformer = dict[key] as Get<QueryValue> | GetSet<any>;
            const mappedValue = typeof transformer === "function" ? value : transformer.set(value);
            return [key, mappedValue];
          })
        )
      })
  );

  return obj;
}

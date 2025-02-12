import { ref } from "vue";
import { fetchAllPages } from "@/composables/use-all-pages";
import type { WaardelijstItem } from "./types";

const urls = {
  organisaties: "/api/v1/organisaties",
  informatiecategorieen: "/api/v1/informatiecategorieen"
} as const;

export const waardelijsten = ref<Record<keyof typeof urls, WaardelijstItem[]>>({
  organisaties: [],
  informatiecategorieen: []
});

export const loadWaardelijsten = async () => {
  try {
    const results = await Promise.allSettled(
      Object.entries(urls).map(([key, url]) =>
        fetchAllPages<WaardelijstItem>(url).then((data) => ({ [key]: data }))
      )
    );

    results.forEach(
      (result) => result.status === "fulfilled" && Object.assign(waardelijsten.value, result.value)
    );
  } catch {
    return;
  }
};

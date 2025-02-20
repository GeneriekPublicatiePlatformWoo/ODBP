import { addToDate, formatIsoDate } from "@/helpers";

type SearchResponse = {
  results: SearchResponseItem[];
  count: number;
  next: boolean;
  previous: boolean;
};

type SearchResponseItem = {
  uuid: string;
  officieleTitel: string;
  resultType: ResultType;
  informatieCategorieen: WaardelijstItem[];
  publisher: WaardelijstItem;
  registratiedatum: string;
  laatstGewijzigdDatum: string;
  omschrijving: string;
};

type WaardelijstItem = {
  uuid: string;
  name: string;
};

export const sortOptions = {
  relevance: { label: "Relevantie", value: "relevance" },
  chronological: { label: "Chronologisch", value: "chronological" }
} as const;

export const resultOptions = {
  publication: { label: "Publicatie", value: "publication" },
  document: { label: "Document", value: "document" }
} as const;

type ValueOf<T> = T[keyof T];
export type Sort = ValueOf<typeof sortOptions>["value"];
export type ResultType = ValueOf<typeof resultOptions>["value"];

export function search({
  signal,
  registratiedatumVanaf,
  registratiedatumTot,
  laatstGewijzigdDatumVanaf,
  laatstGewijzigdDatumTot,
  ...body
}: {
  query: string;
  page: number;
  sort: Sort;
  registratiedatumVanaf?: string | null;
  registratiedatumTot?: string | null;
  laatstGewijzigdDatumVanaf?: string | null;
  laatstGewijzigdDatumTot?: string | null;
  signal?: AbortSignal;
}): Promise<SearchResponse> {
  return fetch("/api/zoeken", {
    body: JSON.stringify({
      ...body,
      registratiedatumVanaf: formatIsoDate(registratiedatumVanaf),
      registratiedatumTot: formatIsoDate(addToDate(registratiedatumTot, { day: 1 })),
      laatstGewijzigdDatumVanaf: formatIsoDate(laatstGewijzigdDatumVanaf),
      laatstGewijzigdDatumTot: formatIsoDate(addToDate(laatstGewijzigdDatumTot, { day: 1 }))
    }),
    method: "POST",
    headers: {
      "content-type": "application/json"
    },
    signal
  })
    .then((r) => (r.ok ? r.json() : Promise.reject(r.status)))
    .catch((reason) => {
      // if the signal is aborted, fetch throws an Error.
      // we don't want to disrupt the user's flow for this.
      if (signal?.aborted) {
        // but we log the reason for debugging purposes
        console.log(reason);
      } else {
        // any other error is unexpected and should disrupt the flow
        return Promise.reject(reason);
      }
    });
}

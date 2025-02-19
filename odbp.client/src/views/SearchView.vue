<template>
  <utrecht-article>
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <div class="zoeken-page">
      <form class="utrecht-form" @submit.prevent="submit">
        <div class="utrecht-form-fieldset zoeken">
          <fieldset
            class="utrecht-form-fieldset__fieldset utrecht-form-fieldset--html-fieldset search-fieldset"
          >
            <utrecht-form-field
              ><utrecht-form-label
                for="92eb76ee-c52f-4dc2-b3db-257ab2cba897"
                aria-hidden="true"
                hidden
                >Zoekterm</utrecht-form-label
              >
              <utrecht-textbox
                id="92eb76ee-c52f-4dc2-b3db-257ab2cba897"
                aria-placeholder="Hier zoeken"
                placeholder="Hier zoeken"
                v-model="zoekVeld"
                type="search"
                autocomplete="off"
                spelcheck="false"
                enterkeyhint="search"
                @search="submit"
            /></utrecht-form-field>
            <utrecht-button type="submit" :appearance="'primary-action-button'"
              >Zoeken</utrecht-button
            >
            <utrecht-form-field
              ><utrecht-form-label for="sort-select" aria-hidden="true" hidden
                >Sorteren</utrecht-form-label
              >
              <utrecht-select
                id="sort-select"
                v-model="sort"
                :options="Object.values(sortOptions)"
              />
              <gpp-woo-icon icon="sort" />
            </utrecht-form-field>
          </fieldset>
        </div>
        <div class="utrecht-form-fieldset filters">
          <!-- <utrecht-heading :level="2">Filters</utrecht-heading> -->
        </div>
      </form>
      <simple-spinner v-if="showSpinner" />
      <p v-else-if="error">Er ging iets mis. Probeer het opnieuw.</p>
      <div v-else-if="data" class="results">
        <div v-if="data.results.length">
          <p>{{ data.count }} gevonden</p>
          <ol>
            <li
              v-for="(
                {
                  uuid,
                  officieleTitel,
                  resultType,
                  informatieCategorieen,
                  publisher,
                  laatstGewijzigdDatum,
                  omschrijving
                },
                idx
              ) in data.results"
              :key="uuid + idx"
            >
              <article>
                <router-link
                  :to="`/${resultType === 'Document' ? 'documenten' : 'publicaties'}/${uuid}`"
                >
                  {{ officieleTitel }}
                </router-link>
                <ul>
                  <li>{{ resultType }}</li>
                  <li v-for="categorie in informatieCategorieen" :key="categorie.uuid">
                    {{ categorie.name }}
                  </li>
                  <li>{{ publisher.name }}</li>
                  <li>{{ laatstGewijzigdDatum }}</li>
                </ul>
                <p>{{ truncate(omschrijving, 200) }}</p>
              </article>
            </li>
          </ol>
          <utrecht-pagination v-if="pagination" v-bind="pagination" />
        </div>
        <p v-else>Geen resultaten gevonden</p>
      </div>
    </div>
  </utrecht-article>
</template>

<script setup lang="ts">
import GppWooIcon from "@/components/GppWooIcon.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import { useLoader } from "@/composables/use-loader";
import { useSpinner } from "@/composables/use-spinner";
import { type Sort, sortOptions, search } from "@/features/search/service";
import { mapPaginatedResultsToUtrechtPagination } from "@/helpers/pagination";
import { useRouteQuery } from "@vueuse/router";
import { computed, ref, watchEffect } from "vue";
import { useRoute, type RouteLocationRaw } from "vue-router";

const route = useRoute();

const first = <T,>(v: T | T[]) => (Array.isArray(v) ? v[0] : v);

const query = useRouteQuery("query", "", {
  transform: (v) => first(v) || ""
});

const page = useRouteQuery("page", 1, {
  transform: (v) => +(first(v) || "1")
});

const sort = useRouteQuery<Sort>("sort", sortOptions.relevance.value, {
  transform: (v) =>
    first(v) === sortOptions.chronological.value
      ? sortOptions.chronological.value
      : sortOptions.relevance.value
});

const zoekVeld = ref("");

watchEffect(() => (zoekVeld.value = query.value || ""));

const submit = () => {
  page.value = 1;
  query.value = zoekVeld.value;
};

const getRoute = (page: number): RouteLocationRaw => ({
  path: route.path,
  query: {
    ...route.query,
    page: page.toString()
  }
});

const { error, loading, data } = useLoader((signal) =>
  search({
    query: query.value,
    page: page.value,
    sort: sort.value,
    signal
  })
);

const showSpinner = useSpinner(loading);

const pagination = computed(
  () =>
    data.value &&
    mapPaginatedResultsToUtrechtPagination({ page: page.value, pagination: data.value, getRoute })
);

const truncate = (s: string, ch: number) => {
  if (s.length <= ch) return s;
  return s.substring(0, ch) + "...";
};
</script>

<style lang="scss" scoped>
.zoeken-page {
  display: grid;
  grid-template-columns: minmax(1) auto;
  grid-template-rows: auto 1fr;
  column-gap: 1rem;

  form {
    display: grid;
    grid-template-columns: subgrid;
    grid-template-rows: subgrid;
    grid-column: 1 / -1;
    grid-row: 1 / -1;
  }

  .zoeken {
    grid-column: 2 / 2;
    grid-row: 1 / 1;
  }

  .filters {
    grid-column: 1 / 1;
    grid-row: 1 / -1;
  }

  .results {
    grid-column: 1 / -1;
    grid-row: 1 / -1;
    display: grid;
    grid-template-columns: subgrid;
    grid-template-rows: subgrid;

    > * {
      grid-column: -1 / -1;
      grid-row: -1 / -1;
    }
  }
}

.search-fieldset {
  --_gap: calc(var(--utrecht-space-inline-md) * 2);
  gap: var(--_gap);
  display: flex;
  align-items: center;

  button {
    margin-inline-start: calc(-2 * var(--utrecht-space-inline-md));
  }

  input {
    max-inline-size: 100%;
    inline-size: 20rem;
  }
}

:has(> #sort-select) {
  display: grid;
  grid-template-columns: 1fr;
  min-inline-size: 16ch;
  align-items: center;
  > * {
    grid-column: 1 / 1;
    grid-row: 1 / 1;
  }
  > :last-child {
    justify-self: end;
    inline-size: 0.5rem;
    display: flex;
    padding-inline-end: var(
      --utrecht-select-padding-inline-end,
      var(--utrecht-form-control-padding-inline-end)
    );
  }
}
</style>

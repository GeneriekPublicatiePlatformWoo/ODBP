<template>
  <utrecht-article>
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <div class="zoeken-page">
      <form class="utrecht-form" @submit.prevent.stop="submit">
        <utrecht-fieldset class="zoeken">
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
            <utrecht-select id="sort-select" v-model="sort" :options="Object.values(sortOptions)" />
            <gpp-woo-icon icon="sort" />
          </utrecht-form-field>
        </utrecht-fieldset>
        <utrecht-fieldset class="filters">
          <!-- <utrecht-legend>Filters</utrecht-legend> -->
          <utrecht-form-field
            ><utrecht-form-label for="registration-date-from"
              >Registratiedatum vanaf</utrecht-form-label
            >
            <utrecht-textbox
              id="registration-date-from"
              v-model="registratiedatumVanaf"
              type="date"
            />
          </utrecht-form-field>
          <utrecht-form-field
            ><utrecht-form-label for="registration-date-until"
              >Registratiedatum tot en met</utrecht-form-label
            >
            <utrecht-textbox
              id="registration-date-until"
              v-model="registratiedatumTot"
              type="date"
            />
          </utrecht-form-field>
        </utrecht-fieldset>
      </form>

      <div class="results">
        <simple-spinner v-if="showSpinner" />
        <p v-else-if="error">Er ging iets mis. Probeer het opnieuw.</p>
        <template v-else-if="data">
          <div v-if="data.results.length">
            <p class="result-count">{{ data.count }} gevonden</p>
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
                <article class="search-result">
                  <utrecht-heading :level="3">
                    <router-link
                      :to="`/${resultType === resultOptions.document.value ? 'documenten' : 'publicaties'}/${uuid}`"
                    >
                      {{ officieleTitel }}
                    </router-link>
                  </utrecht-heading>
                  <ul>
                    <li>
                      <strong>{{ resultOptions[resultType].label }}</strong>
                    </li>
                    <li>{{ publisher.name }}</li>
                    <li v-for="categorie in informatieCategorieen" :key="categorie.uuid">
                      {{ categorie.name }}
                    </li>
                  </ul>
                  <p>{{ truncate(omschrijving, 150) }}</p>
                  <time :datetime="laatstGewijzigdDatum">{{
                    formatDate(laatstGewijzigdDatum)
                  }}</time>
                </article>
              </li>
            </ol>
            <utrecht-pagination v-if="pagination" v-bind="pagination" class="pagination" />
          </div>
          <p v-else>Geen resultaten gevonden</p>
        </template>
      </div>
    </div>
  </utrecht-article>
</template>

<script setup lang="ts">
import GppWooIcon from "@/components/GppWooIcon.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import { useLoader } from "@/composables/use-loader";
import { useRouterQuerySingle } from "@/composables/use-router-query";
import { useSpinner } from "@/composables/use-spinner";
import { sortOptions, search, resultOptions } from "@/features/search/service";
import { formatDate } from "@/helpers";
import { mapPaginatedResultsToUtrechtPagination } from "@/helpers/pagination";
import { computed, ref, watchEffect } from "vue";
import { useRoute, type RouteLocationRaw } from "vue-router";

const route = useRoute();

const query = useRouterQuerySingle("query", (x) => x || "");
const page = useRouterQuerySingle("page", (x) => +(x || "1"));
const sort = useRouterQuerySingle("sort", (x) =>
  x === sortOptions.chronological.value
    ? sortOptions.chronological.value
    : sortOptions.relevance.value
);

const registratiedatumVanaf = useRouterQuerySingle("registratiedatumVanaf", (x) => x || "");

const registratiedatumTot = useRouterQuerySingle("registratiedatumTot", (x) => x || "");

const zoekVeld = ref("");

watchEffect(() => (zoekVeld.value = query.value));

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
    registratiedatumVanaf: registratiedatumVanaf.value || undefined,
    registratiedatumTot: registratiedatumTot.value || undefined,
    signal
  })
);

const showSpinner = useSpinner(loading);

const pagination = computed(
  () =>
    data.value &&
    mapPaginatedResultsToUtrechtPagination({
      page: page.value,
      pagination: data.value,
      getRoute
    })
);

const truncate = (s: string, ch: number) => {
  if (s.length <= ch) return s;
  return s.substring(0, ch) + "...";
};
</script>

<style lang="scss" scoped>
.zoeken-page {
  display: grid;
  grid-template-columns: minmax(14rem, 1fr) minmax(var(--utrecht-article-max-inline-size), 1fr);
  grid-template-rows: auto 1fr;
  column-gap: calc(2 * var(--utrecht-space-inline-md));

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
    z-index: 1;
  }

  .filters {
    grid-column: 1 / 1;
    grid-row: 1 / -1;
    z-index: 1;

    > :first-child {
      display: grid;
      gap: 0.5rem;
    }
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
      display: flex;
      flex-direction: column;
    }
  }
}

.zoeken > :first-child {
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

ol,
ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

article.search-result {
  ul {
    display: flex;
    column-gap: var(--utrecht-space-inline-xs);
    flex-wrap: wrap;
  }
  li,
  time {
    font-size: 0.75em;
  }
  p {
    margin: 0;
  }
}

.pagination {
  margin-inline: auto;
  margin-block-start: var(--utrecht-space-inline-md);
}

.result-count {
  margin: 0;
}
</style>

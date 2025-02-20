<template>
  <div class="zoeken-page">
    <utrecht-heading :level="1">Zoeken</utrecht-heading>
    <form class="utrecht-form" @submit.prevent.stop="submit">
      <utrecht-fieldset class="zoeken">
        <utrecht-form-field
          ><utrecht-form-label for="search-field">Zoekterm</utrecht-form-label>
          <div class="search-bar">
            <utrecht-textbox
              id="search-field"
              aria-placeholder="Hier zoeken"
              placeholder="Hier zoeken"
              v-model="formFields.query"
              type="search"
              autocomplete="off"
              spelcheck="false"
              enterkeyhint="search"
              @search="submit"
            />
            <utrecht-button type="submit" :appearance="'primary-action-button'"
              >Zoeken</utrecht-button
            >
          </div>
        </utrecht-form-field>

        <utrecht-form-field
          ><utrecht-form-label for="sort-select">Sorteren</utrecht-form-label>
          <div>
            <utrecht-select
              id="sort-select"
              v-model="queryParams.sort"
              :options="Object.values(sortOptions)"
            />
            <gpp-woo-icon icon="sort" />
          </div>
        </utrecht-form-field>
      </utrecht-fieldset>
      <utrecht-fieldset class="filters">
        <utrecht-legend class="visually-hidden">Filters</utrecht-legend>
        <utrecht-form-field
          ><utrecht-form-label for="registration-date-from"
            >Registratiedatum vanaf</utrecht-form-label
          >
          <utrecht-textbox
            id="registration-date-from"
            v-model="formFields.registratiedatumVanaf"
            type="date"
          />
        </utrecht-form-field>
        <utrecht-form-field
          ><utrecht-form-label for="registration-date-until"
            >Registratiedatum tot en met</utrecht-form-label
          >
          <utrecht-textbox
            id="registration-date-until"
            v-model="formFields.registratiedatumTot"
            type="date"
          />
        </utrecht-form-field>
        <utrecht-form-field
          ><utrecht-form-label for="update-date-from">Wijzigingsdatum vanaf</utrecht-form-label>
          <utrecht-textbox
            id="updated-date-from"
            v-model="formFields.laatstGewijzigdDatumVanaf"
            type="date"
          />
        </utrecht-form-field>
        <utrecht-form-field
          ><utrecht-form-label for="updated-date-until"
            >Wijzigingsdatum tot en met</utrecht-form-label
          >
          <utrecht-textbox
            id="updated-date-until"
            v-model="formFields.laatstGewijzigdDatumTot"
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
          <p class="result-count">{{ data.count }} resultaten gevonden</p>
          <ol>
            <li
              v-for="(
                {
                  uuid,
                  officieleTitel,
                  resultType,
                  informatieCategorieen,
                  publisher,
                  registratiedatum,
                  laatstGewijzigdDatum,
                  omschrijving
                },
                idx
              ) in data.results"
              :key="uuid + idx"
            >
              <utrecht-article class="search-result">
                <utrecht-heading :level="3">
                  <router-link
                    :to="`/${resultType === resultOptions.document.value ? 'documenten' : 'publicaties'}/${uuid}`"
                  >
                    {{ officieleTitel }}
                  </router-link>
                </utrecht-heading>
                <ul>
                  <li class="result-type">
                    <strong>{{ resultOptions[resultType].label }}</strong>
                  </li>
                  <li class="publisher">{{ publisher.name }}</li>
                  <li
                    class="category"
                    v-for="categorie in informatieCategorieen"
                    :key="categorie.uuid"
                  >
                    {{ categorie.name }}
                  </li>
                </ul>
                <utrecht-paragraph>{{ truncate(omschrijving, 150) }}</utrecht-paragraph>

                <p>
                  <time :datetime="registratiedatum">{{ formatDate(registratiedatum) }}</time>
                  <template
                    v-if="
                      laatstGewijzigdDatum?.substring(0, 10) !== registratiedatum?.substring(0, 10)
                    "
                    ><span>{{ ", gewijzigd op " }}</span
                    ><time :datetime="laatstGewijzigdDatum">{{
                      formatDate(laatstGewijzigdDatum)
                    }}</time>
                  </template>
                </p>
              </utrecht-article>
            </li>
          </ol>
          <utrecht-pagination v-if="pagination" v-bind="pagination" class="pagination" />
        </div>
        <p v-else>Geen resultaten gevonden</p>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import GppWooIcon from "@/components/GppWooIcon.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import { useLoader } from "@/composables/use-loader";
import { useQueryValues, type GetSet } from "@/composables/use-router-query";
import { useSpinner } from "@/composables/use-spinner";
import { sortOptions, search, resultOptions } from "@/features/search/service";
import { formatDate } from "@/helpers";
import { mapPaginatedResultsToUtrechtPagination } from "@/helpers/pagination";
import { truncate } from "@/helpers/strings";
import { computed, onMounted, reactive } from "vue";
import { useRoute, type RouteLocationRaw } from "vue-router";

const route = useRoute();
const single = <T,>(v: T[]) => v[0];

const singleNumber: GetSet<number> = {
  get: (x) => +(x[0] || "1"),
  set: (x) => x.toString()
};

const queryParams = useQueryValues({
  query: single,
  registratiedatumVanaf: single,
  registratiedatumTot: single,
  laatstGewijzigdDatumVanaf: single,
  laatstGewijzigdDatumTot: single,
  sort: (x) =>
    x[0] === sortOptions.chronological.value
      ? sortOptions.chronological.value
      : sortOptions.relevance.value,
  page: singleNumber
});

const formFields = reactive({
  query: "",
  registratiedatumVanaf: "",
  registratiedatumTot: "",
  laatstGewijzigdDatumVanaf: "",
  laatstGewijzigdDatumTot: ""
});

onMounted(() => {
  const keys = Object.keys(formFields) as Array<keyof typeof formFields>;
  keys.forEach((key) => (formFields[key] = queryParams[key]));
});

const submit = () => {
  Object.assign(queryParams, {
    ...formFields,
    page: 1
  });
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
    ...queryParams,
    signal
  })
);

const showSpinner = useSpinner(loading);

const pagination = computed(
  () =>
    data.value &&
    mapPaginatedResultsToUtrechtPagination({
      page: queryParams.page,
      pagination: data.value,
      getRoute
    })
);
</script>

<style lang="scss" scoped>
.zoeken-page {
  display: grid;
  grid-template-columns: minmax(auto, 14rem) 1fr;
  grid-template-rows: auto auto 1fr;
  column-gap: clamp(var(--utrecht-space-inline-md), 3vw, var(--utrecht-space-inline-4xl));

  > .utrecht-heading-1 {
    grid-row: 1 / 1;
    grid-column: 2 / 2;
    z-index: 1;
  }

  form {
    display: grid;
    grid-template-columns: subgrid;
    grid-template-rows: subgrid;
    grid-column: 1 / -1;
    grid-row: 1 / -1;
  }

  .zoeken {
    grid-column: 2 / 2;
    grid-row: 2 / 2;
    z-index: 1;
  }

  .filters {
    grid-column: 1 / 1;
    grid-row: 2 / -1;
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

input {
  block-size: 2.5rem; // date and text inputs are not the same height otherwise...
}

.zoeken > :first-child {
  column-gap: calc(var(--utrecht-space-inline-md) * 2);
  display: flex;
  align-items: end;
  flex-wrap: wrap;

  input {
    max-inline-size: 100%;
    inline-size: 20rem;
  }

  .search-bar {
    display: flex;
    flex-wrap: wrap;
  }
}

:has(> #sort-select) {
  min-inline-size: 16ch;
  position: relative;

  > :last-child {
    position: absolute;
    inset-inline-end: 0;
    block-size: 100%;
    inline-size: 0.5rem;
    display: flex;
    padding-inline-end: var(
      --utrecht-select-padding-inline-end,
      var(--utrecht-form-control-padding-inline-end)
    );
    pointer-events: none;
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
    row-gap: var(--utrecht-space-inline-2xs);
    align-items: center;
    flex-wrap: wrap;
  }
  li,
  :has(time) {
    font-size: 0.75em;
  }
  .category {
    border-bottom: 1px dotted lightgray;
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

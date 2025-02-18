<template>
  <utrecht-article>
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <form class="utrecht-form" @submit.prevent="submit">
      <div class="utrecht-form-fieldset">
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
        </fieldset>
      </div>
      <div class="utrecht-form-fieldset">
        <fieldset class="utrecht-form-fieldset__fieldset utrecht-form-fieldset--html-fieldset">
          <utrecht-form-field
            ><utrecht-form-label for="sort-select" aria-hidden="true" hidden
              >Sorteren</utrecht-form-label
            >
            <utrecht-select id="sort-select" v-model="sort" :options="Object.values(sortOptions)" />
          </utrecht-form-field>
        </fieldset>
      </div>
    </form>

    <simple-spinner v-if="showSpinner" />

    <p v-else-if="error">Er ging iets mis. Probeer het opnieuw.</p>

    <div v-else-if="data">
      <template v-if="data.results.length">
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
        <utrecht-pagination :pagination="data" :page="page" :get-route="getRoute" />
      </template>
      <p v-else>Geen resultaten gevonden</p>
    </div>
  </utrecht-article>
</template>

<script setup lang="ts">
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import { useLoader } from "@/composables/use-loader";
import { type Sort, sortOptions, search } from "@/features/search/service";
import { useRouteQuery } from "@vueuse/router";
import { ref } from "vue";
import { useRoute, type RouteLocationRaw } from "vue-router";

const route = useRoute();

const query = useRouteQuery<string | null>("query", null);
const page = useRouteQuery("page", "1", { transform: Number });
const sort = useRouteQuery<Sort>("sort", sortOptions.relevance.value, {
  transform: (v) => (v === sortOptions.chronological.value ? v : sortOptions.relevance.value)
});

const zoekVeld = ref(query.value || "");

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

const { error, showSpinner, data } = useLoader((signal) => {
  if (typeof query.value === "string")
    return search({
      query: query.value,
      page: page.value,
      sort: sort.value,
      signal
    });
});

const truncate = (s: string, ch: number) => {
  if (s.length <= ch) return s;
  return s.substring(0, ch) + "...";
};
</script>

<style lang="scss" scoped>
.search-fieldset {
  display: flex;
  align-items: center;

  input {
    max-inline-size: 100%;
    inline-size: 20rem;
  }
}
</style>

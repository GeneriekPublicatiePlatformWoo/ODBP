<template>
  <utrecht-article>
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <form class="utrecht-form" @submit.prevent="search">
      <div class="utrecht-form-fieldset">
        <fieldset class="utrecht-form-fieldset__fieldset utrecht-form-fieldset--html-fieldset">
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
          /></utrecht-form-field>
        </fieldset>
      </div>
    </form>

    <SimpleSpinner v-if="loading" />

    <div v-else-if="showResults">
      <template v-if="response?.results?.length">
        <p>{{ response.count }} gevonden</p>
        <ol>
          <li v-for="{ uuid, officieleTitel, resultType } in response.results" :key="uuid">
            <router-link
              :to="`/${resultType === 'Document' ? 'documenten' : 'publicaties'}/${uuid}`"
            >
              {{ officieleTitel }}
            </router-link>
          </li>
        </ol>
        <UtrechtPagination :pagination="response" v-model:page="page" :get-route="getRoute" />
      </template>
      <p v-else>Geen resultaten gevonden</p>
    </div>
  </utrecht-article>
</template>

<script setup lang="ts">
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import { computed, ref, watchEffect } from "vue";
import { useRoute, type RouteLocationRaw } from "vue-router";

let controller: AbortController | null = null;

const route = useRoute();

const query = computed({
  get: () => (typeof route.query.query === "string" ? route.query.query : undefined),
  set: (v) => (route.query.query = v || [])
});

const page = computed({
  get: () => (typeof route.query.page === "string" ? +route.query.page : 1),
  set: (v) => (route.query.page = v.toString())
});

const zoekVeld = ref<string>(query.value || "");
const loading = ref(false);
const error = ref(false);
const response = ref();

const showResults = ref(false);

const search = () => {
  query.value = zoekVeld.value;
  page.value = 1;
};

const getRoute = (page: number): RouteLocationRaw => ({
  path: route.path,
  query: {
    ...route.query,
    page: page.toString()
  }
});

watchEffect(() => {
  if (query.value === undefined) return;
  controller?.abort();
  controller = new AbortController();
  const setLoadingTimeout = setTimeout(() => {
    loading.value = true;
  }, 200);
  controller.signal.addEventListener("abort", () => clearTimeout(setLoadingTimeout));
  error.value = false;
  fetch(`/api/zoeken?${new URLSearchParams({ query: query.value, page: page.value.toString() })}`, {
    signal: controller.signal
  })
    .finally(() => clearTimeout(setLoadingTimeout))
    .then((r) => (r.ok ? r.json() : Promise.reject(r.status)))
    .then((r) => {
      showResults.value = true;
      response.value = r;
    })
    .catch(() => (error.value = true))
    .finally(() => (loading.value = false));
});
</script>

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

    <p v-else-if="error">Er ging iets mis. Probeer het opnieuw.</p>

    <div v-else-if="showResults">
      <template v-if="response?.results?.length">
        <p>{{ response.count }} gevonden</p>
        <ol>
          <li
            v-for="({ uuid, officieleTitel, resultType }, idx) in response.results"
            :key="uuid + idx"
          >
            <router-link
              :to="`/${resultType === 'Document' ? 'documenten' : 'publicaties'}/${uuid}`"
            >
              {{ officieleTitel }}
            </router-link>
          </li>
        </ol>
        <UtrechtPagination :pagination="response" :page="page" :get-route="getRoute" />
      </template>
      <p v-else>Geen resultaten gevonden</p>
    </div>
  </utrecht-article>
</template>

<script setup lang="ts">
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import { useRouteQuery } from "@vueuse/router";
import { ref, watchEffect } from "vue";
import { useRoute, type RouteLocationRaw } from "vue-router";

let controller: AbortController | null = null;

const route = useRoute();

const query = useRouteQuery<string | null>("query", null);
const page = useRouteQuery("page", "1", { transform: Number });

const zoekVeld = ref<string>(query.value || "");
const loading = ref(false);
const error = ref(false);
const response = ref();

const showResults = ref(false);

const search = () => {
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

watchEffect(() => {
  if (typeof query.value !== "string") return;

  if (controller) {
    controller.abort();
    controller.signal.onabort = null;
  }

  controller = new AbortController();
  const signal = controller.signal;

  const setLoadingTimeout = setTimeout(() => {
    loading.value = true;
  }, 200);
  const clearLoadingTimeout = () => clearTimeout(setLoadingTimeout);
  signal.onabort = clearLoadingTimeout;

  error.value = false;

  fetch(
    `/api/zoeken?${new URLSearchParams([
      ["query", query.value],
      ["page", page.value.toString()]
    ])}`,
    {
      signal
    }
  )
    .finally(clearLoadingTimeout)
    .then((r) => (r.ok ? r.json() : Promise.reject(r.status)))
    .then((r) => {
      showResults.value = true;
      response.value = r;
    })
    .catch((reason) => {
      if (!signal.aborted) {
        error.value = true;
      } else {
        console.log(reason);
      }
    })
    .finally(() => (loading.value = false));
});
</script>

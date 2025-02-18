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

    <simple-spinner v-if="loading" />

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
        <utrecht-pagination :pagination="response" :page="page" :get-route="getRoute" />
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
    // cancel any in-flight fetch requests
    // because we're only interested in the latest result
    controller.abort();
  }

  controller = new AbortController();
  const signal = controller.signal;

  // only show a spinner if the request takes longer than 200 ms.
  // otherwise, we don't want to disrupt the users's flow
  const setLoadingTimeout = setTimeout(() => {
    loading.value = true;
  }, 200);
  const clearLoadingTimeout = () => clearTimeout(setLoadingTimeout);
  signal.addEventListener("abort", clearLoadingTimeout);

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
    .then((r) => (r.ok ? r.json() : Promise.reject(r.status)))
    .then((r) => {
      showResults.value = true;
      response.value = r;
    })
    .catch((reason) => {
      // if the signal is aborted, fetch throws an Error.
      // we don't want to disrupt the user's flow for this.
      if (signal.aborted) {
        // but we log the reason for debugging purposes
        console.log(reason);
      } else {
        // any other error is unexpected and should disrupt the flow
        error.value = true;
      }
    })
    .finally(() => {
      clearLoadingTimeout();
      loading.value = false;
    });
});
</script>

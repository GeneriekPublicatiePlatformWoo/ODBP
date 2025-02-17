<template>
  <utrecht-article>
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <form class="utrecht-form" @submit.prevent="search">
      <div class="utrecht-form-fieldset">
        <fieldset class="utrecht-form-fieldset__fieldset utrecht-form-fieldset--html-fieldset">
          <utrecht-form-field :required="true"
            ><utrecht-form-label
              for="92eb76ee-c52f-4dc2-b3db-257ab2cba897"
              aria-hidden="true"
              hidden
              >Zoekterm</utrecht-form-label
            >
            <utrecht-textbox
              id="92eb76ee-c52f-4dc2-b3db-257ab2cba897"
              :required="true"
              aria-placeholder="Hier zoeken"
              placeholder="Hier zoeken"
              v-model="zoekTerm"
          /></utrecht-form-field>
        </fieldset>
      </div>
    </form>
    {{ results }}
  </utrecht-article>
</template>

<script setup lang="ts">
import { ref } from "vue";

let controller: AbortController | null = null;
const zoekTerm = ref<string>("");
const results = ref<any[]>([]);
const loading = ref(false);
const error = ref(false);

const search = () => {
  controller?.abort();
  controller = new AbortController();
  loading.value = true;
  error.value = false;
  fetch(`/api/zoeken?${new URLSearchParams({ query: zoekTerm.value })}`, {
    signal: controller.signal
  })
    .then((r) => (r.ok ? r.json() : Promise.reject(r.status)))
    .then((r) => (results.value = r.results))
    .catch(() => (error.value = true))
    .finally(() => (loading.value = false));
};
</script>

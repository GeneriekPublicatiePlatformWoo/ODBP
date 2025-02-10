<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <alert-inline v-else-if="error"
    >Er is iets misgegaan bij het ophalen van de publicatie...</alert-inline
  >

  <section v-else>
    <utrecht-heading :level="1">{{ publicatie?.officieleTitel }}</utrecht-heading>

    <utrecht-table>
      <utrecht-table-caption>Over deze publicatie</utrecht-table-caption>
      <utrecht-table-header class="utrecht-table__header--hidden">
        <utrecht-table-row>
          <utrecht-table-header-cell scope="col">Publicatie kenmerk</utrecht-table-header-cell>
          <utrecht-table-header-cell scope="col"
            >Publicatie kenmerkwaarde</utrecht-table-header-cell
          >
        </utrecht-table-row>
      </utrecht-table-header>
      <utrecht-table-body>
        <utrecht-table-row v-for="[key, value] in publicatieRows" :key="key">
          <template v-if="value?.length">
            <utrecht-table-header-cell scope="row">{{ key }}</utrecht-table-header-cell>
            <utrecht-table-cell>{{ value }}</utrecht-table-cell>
          </template>
        </utrecht-table-row>
      </utrecht-table-body>
    </utrecht-table>
  </section>
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useFetchApi } from "@/api/use-fetch-api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import AlertInline from "@/components/AlertInline.vue";
import type { Publicatie } from "./types";

const API_URL = `/api/v1`;

const props = defineProps<{ uuid: string }>();

const {
  get: getPublicatie,
  data: publicatie,
  isFetching: loading,
  error: error
} = useFetchApi(() => `${API_URL}/publicaties/${props.uuid}`, {
  immediate: false
}).json<Publicatie>();

const publicatieRows = computed<Map<string, string | undefined>>(
  () =>
    new Map([
      ["Informatiecategorieën", publicatie.value?.informatieCategorieen.join(", ")],
      ["Organisatie", publicatie.value?.publisher],
      ["Officiële titel", publicatie.value?.officieleTitel],
      ["Verkorte titel", publicatie.value?.verkorteTitel],
      ["Omschrijving", publicatie.value?.omschrijving],
      [
        "Geregistreerd op",
        publicatie.value?.registratiedatum &&
          Intl.DateTimeFormat("default", { dateStyle: "long" }).format(
            Date.parse(publicatie.value.registratiedatum)
          )
      ],
      ["Laatst gewijzigd op ??", publicatie.value?.registratiedatum]
    ])
);

onMounted(async () => props.uuid && (await getPublicatie().execute()));
</script>

<style lang="scss" scoped></style>

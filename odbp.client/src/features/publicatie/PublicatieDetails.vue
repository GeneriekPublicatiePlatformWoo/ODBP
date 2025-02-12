<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <alert-inline v-else-if="error"
    >Er is iets misgegaan bij het ophalen van de publicatie...</alert-inline
  >

  <template v-else>
    <utrecht-heading :level="1">{{ publicatieData?.officieleTitel }}</utrecht-heading>

    <section>
      <utrecht-table>
        <utrecht-table-caption>Over deze publicatie</utrecht-table-caption>

        <utrecht-table-header class="utrecht-table__header--hidden">
          <utrecht-table-row>
            <utrecht-table-header-cell scope="col">Publicatiekenmerk</utrecht-table-header-cell>
            <utrecht-table-header-cell scope="col"
              >Publicatiekenmerkwaarde</utrecht-table-header-cell
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

      <utrecht-heading :level="2" :id="headingId">Documenten bij deze publicatie</utrecht-heading>

      <utrecht-table :aria-labelledby="headingId" class="utrecht-table--alternate-row-color">
        <utrecht-table-header>
          <utrecht-table-row>
            <utrecht-table-header-cell scope="col">Officiële titel</utrecht-table-header-cell>
            <utrecht-table-header-cell scope="col">Laatst gewijzigd op</utrecht-table-header-cell>
            <utrecht-table-header-cell scope="col">Download</utrecht-table-header-cell>
          </utrecht-table-row>
        </utrecht-table-header>

        <utrecht-table-body>
          <utrecht-table-row
            v-for="{ uuid, officieleTitel, laatstGewijzigdDatum, bestandsnaam } in documenten"
            :key="uuid"
          >
            <utrecht-table-cell>{{ officieleTitel }}</utrecht-table-cell>
            <utrecht-table-cell>{{ formatDate(laatstGewijzigdDatum) }}</utrecht-table-cell>
            <utrecht-table-cell>
              <utrecht-link
                :href="`${API_URL}/documenten/${uuid}/download`"
                :download="bestandsnaam"
                >{{ bestandsnaam }}</utrecht-link
              >
            </utrecht-table-cell>
          </utrecht-table-row>
        </utrecht-table-body>
      </utrecht-table>
    </section>
  </template>
</template>

<script setup lang="ts">
import { computed, useId } from "vue";
import { useFetchApi } from "@/api/use-fetch-api";
import { useAllPages } from "@/composables/use-all-pages";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import AlertInline from "@/components/AlertInline.vue";
import { formatDate } from "@/helpers";
import type { Publicatie, PublicatieDocument } from "./types";
import { waardelijsten } from "@/stores/waardelijsten";

const API_URL = `/api/v1`;

const props = defineProps<{ uuid: string }>();

const headingId = useId();

const loading = computed(() => loadingPublicatie.value || loadingDocumenten.value);
const error = computed(() => !!publicatieError.value || !!documentenError.value);

const {
  data: publicatieData,
  isFetching: loadingPublicatie,
  error: publicatieError
} = useFetchApi(() => `${API_URL}/publicaties/${props.uuid}`).json<Publicatie>();

const {
  data: documenten,
  loading: loadingDocumenten,
  error: documentenError
} = useAllPages<PublicatieDocument>(`${API_URL}/documenten/?publicatie=${props.uuid}`);

const publicatieRows = computed<Map<string, string | undefined>>(
  () =>
    new Map([
      [
        "Informatiecategorieën",
        publicatieData.value?.informatieCategorieen
          .map(
            (uuid) =>
              waardelijsten.value.informatiecategorieen.find((c) => c.uuid === uuid)?.naam ||
              "onbekend"
          )
          .join(", ")
      ],
      [
        "Organisatie",
        waardelijsten.value.organisaties.find((o) => o.uuid === publicatieData.value?.publisher)
          ?.naam || "onbekend"
      ],
      ["Officiële titel", publicatieData.value?.officieleTitel],
      ["Verkorte titel", publicatieData.value?.verkorteTitel],
      ["Omschrijving", publicatieData.value?.omschrijving],
      ["Geregistreerd op", formatDate(publicatieData.value?.registratiedatum)],
      ["Laatst gewijzigd op", formatDate(publicatieData.value?.laatstGewijzigdDatum)]
    ])
);
</script>

<style lang="scss" scoped>
th[scope="row"] {
  inline-size: 20ch;
}
</style>

<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <alert-inline v-else-if="error"
    >Er is iets misgegaan bij het ophalen van de documentgegevens...</alert-inline
  >

  <template v-else>
    <utrecht-heading :level="1">{{ documentData?.officieleTitel }}</utrecht-heading>

    <section>
      <utrecht-table>
        <utrecht-table-caption>Over dit document</utrecht-table-caption>

        <utrecht-table-header class="utrecht-table__header--hidden">
          <utrecht-table-row>
            <utrecht-table-header-cell scope="col">Documentkenmerk</utrecht-table-header-cell>
            <utrecht-table-header-cell scope="col">Documentkenmerkwaarde</utrecht-table-header-cell>
          </utrecht-table-row>
        </utrecht-table-header>

        <utrecht-table-body>
          <utrecht-table-row v-for="[key, value] in documentRows" :key="key">
            <template v-if="value?.length">
              <utrecht-table-header-cell scope="row">{{ key }}</utrecht-table-header-cell>
              <utrecht-table-cell>{{ value }}</utrecht-table-cell>
            </template>
          </utrecht-table-row>
        </utrecht-table-body>
      </utrecht-table>

      <utrecht-paragraph>
        <a
          :href="`${API_URL}/documenten/${uuid}/download`"
          :download="documentData?.bestandsnaam"
          class="utrecht-button-link utrecht-button-link--html-a utrecht-button-link--primary-action"
        >
          <gpp-woo-icon icon="download" />

          Download ({{ documentData?.bestandsnaam.split(".").pop()
          }}{{
            documentData?.bestandsomvang
              ? `, ${Math.floor(documentData.bestandsomvang / 1024)}kb`
              : ""
          }})
        </a>
      </utrecht-paragraph>

      <utrecht-heading :level="2" :id="headingId">Gekoppelde publicatie</utrecht-heading>

      <utrecht-table :aria-labelledby="headingId">
        <utrecht-table-header class="utrecht-table__header--hidden">
          <utrecht-table-row>
            <utrecht-table-header-cell scope="col">Publicatiekenmerk</utrecht-table-header-cell>
            <utrecht-table-header-cell scope="col"
              >Publicatiekenmerkwaarde</utrecht-table-header-cell
            >
          </utrecht-table-row>
        </utrecht-table-header>

        <utrecht-table-body>
          <utrecht-table-row>
            <utrecht-table-header-cell scope="row">Officiële titel</utrecht-table-header-cell>
            <utrecht-table-cell>
              <router-link
                :to="{ name: 'publicatie', params: { uuid: publicatieData?.uuid } }"
                class="utrecht-link utrecht-link--html-a"
                >{{ publicatieData?.officieleTitel }}</router-link
              >
            </utrecht-table-cell>
          </utrecht-table-row>

          <utrecht-table-row>
            <utrecht-table-header-cell scope="row">Laatst gewijzigd op</utrecht-table-header-cell>
            <utrecht-table-cell>{{
              formatDate(publicatieData?.laatstGewijzigdDatum)
            }}</utrecht-table-cell>
          </utrecht-table-row>
        </utrecht-table-body>
      </utrecht-table>
    </section>
  </template>
</template>

<script setup lang="ts">
import { computed, useId, watch } from "vue";
import { useFetchApi } from "@/api/use-fetch-api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import AlertInline from "@/components/AlertInline.vue";
import GppWooIcon from "@/components/GppWooIcon.vue";
import { formatDate } from "@/helpers";
import type { Publicatie, PublicatieDocument } from "./types";
import { waardelijsten } from "@/stores/waardelijsten";

const API_URL = `/api/v1`;

const props = defineProps<{ uuid: string }>();

const headingId = useId();

const loading = computed(() => loadingDocument.value || loadingPublicatie.value);
const error = computed(() => !!documentError.value || !!publicatieError.value);

const {
  data: documentData,
  isFetching: loadingDocument,
  error: documentError
} = useFetchApi(() => `${API_URL}/documenten/${props.uuid}`).json<PublicatieDocument>();

const {
  get: getPublicatie,
  data: publicatieData,
  isFetching: loadingPublicatie,
  error: publicatieError
} = useFetchApi(() => `${API_URL}/publicaties/${documentData.value?.publicatie}`, {
  immediate: false
}).json<Publicatie>();

watch(
  () => documentData.value?.publicatie,
  async (publicatie) => publicatie && (await getPublicatie().execute())
);

const documentRows = computed<Map<string, string | undefined>>(
  () =>
    new Map([
      ["Identificatie", documentData.value?.identifier],
      ["Officiële titel", documentData.value?.officieleTitel],
      ["Verkorte titel", documentData.value?.verkorteTitel],
      ["Omschrijving", documentData.value?.omschrijving],
      [
        "Eigenaar",
        waardelijsten.value.organisaties.find((o) => o.uuid === publicatieData.value?.publisher)
          ?.naam || "onbekend"
      ],
      ["Geregistreerd op", formatDate(documentData.value?.registratiedatum)],
      ["Laatst gewijzigd op", formatDate(documentData.value?.laatstGewijzigdDatum)]
    ])
);
</script>

<style lang="scss" scoped>
th[scope="row"] {
  inline-size: 20ch;
}
</style>

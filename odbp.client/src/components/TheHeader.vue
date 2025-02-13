<template>
  <utrecht-page-header>
    <utrecht-skip-link href="#main">Naar inhoud</utrecht-skip-link>

    <utrecht-skip-link href="#menu">Naar menu</utrecht-skip-link>

    <router-link
      :to="{ name: 'home' }"
      class="utrecht-link utrecht-link--html-a utrecht-link--box-content"
    >
      <img
        v-if="resources?.logoUrl"
        :src="resources.logoUrl"
        alt="Logo"
        height="100"
        crossorigin="anonymous"
      />
    </router-link>

    <the-nav />
  </utrecht-page-header>
</template>

<script lang="ts" setup>
import TheNav from "./TheNav.vue";
import { injectResources } from "@/resources";

const resources = injectResources();
</script>

<template>
  <utrecht-page-header>
    <utrecht-skip-link href="#main">Naar inhoud</utrecht-skip-link>

    <utrecht-skip-link href="#menu">Naar menu</utrecht-skip-link>

    <router-link :to="{ name: 'home' }">
      <img v-if="resources?.logo" :src="resources.logo" alt="Logo" height="100" crossorigin="anonymous" />
    </router-link>

    <the-nav />
  </utrecht-page-header>
</template>

<script lang="ts" setup>
import TheNav from "./TheNav.vue";
import { injectResources } from "@/resources";

const resources = injectResources();
</script>

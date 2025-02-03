<template>
  <header>
    <utrecht-skip-link href="#main">Naar inhoud</utrecht-skip-link>

    <utrecht-skip-link href="#menu">Naar menu</utrecht-skip-link>

    <router-link :to="{ name: 'home' }">
      <img v-if="resources?.logo" :src="resources.logo" alt="Logo" height="100" />
    </router-link>

    <the-nav />
  </header>
</template>

<script lang="ts" setup>
import { inject } from "vue";
import TheNav from "./TheNav.vue";
import type { Resources } from "@/resources";

const resources = inject<Resources>("resources");
</script>

<style lang="scss" scoped>
img {
  display: block;
}
</style>

<template>
  <the-header />

  <main id="main">
    <the-hero />

    <section class="utrecht-page utrecht-page-content">
      <router-view />
    </section>
  </main>

  <the-footer />
</template>

<script setup lang="ts">
import { RouterView } from "vue-router";
import TheHeader from "./components/TheHeader.vue";
import TheHero from "./components/TheHero.vue";
import TheFooter from "./components/TheFooter.vue";
</script>

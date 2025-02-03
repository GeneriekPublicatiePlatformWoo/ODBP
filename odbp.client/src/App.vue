<script setup lang="ts">
import { RouterView } from "vue-router";
import TheHeader from "./components/TheHeader.vue";
import TheFooter from "./components/TheFooter.vue";
</script>

<template>
  <the-header />

  <main id="main">
    <router-view />
  </main>

  <the-footer />
</template>

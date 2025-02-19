<template>
  <section class="gpp-woo-hero">
    <div class="utrecht-page utrecht-page-content">
      <img
        v-if="resources?.imageUrl"
        :src="resources.imageUrl"
        class="gpp-woo-hero-image"
        alt="Afbeelding gemeente"
        crossorigin="anonymous"
      />

      <div class="gpp-woo-card">
        <div class="gpp-woo-card-content">
          <span class="utrecht-heading-2"
            >Open {{ naam }}, zoek hier in openbaar gemaakte informatie van Gemeente
            {{ naam }}</span
          >

          <utrecht-paragraph v-if="route.name === 'home'">
            <router-link
              :to="{ name: 'zoeken' }"
              class="utrecht-button-link utrecht-button-link--html-a utrecht-button-link--primary-action"
              >Zoeken</router-link
            >
          </utrecht-paragraph>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { injectResources } from "@/resources";
import { useRoute } from "vue-router";

const resources = injectResources();
const naam = computed(() => (resources?.name ? resources.name : "-"));
const route = useRoute();
</script>

<style lang="scss" scoped>
.gpp-woo-hero {
  position: relative;

  .gpp-woo-hero-image {
    position: absolute;
    inset: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .gpp-woo-card {
    position: relative;
  }
}

.gpp-woo-card {
  inline-size: min(100%, var(--gpp-woo-card-max-inline-size));
  background-color: var(--gpp-woo-card-background-color);

  .gpp-woo-card-content {
    padding-block: var(--gpp-woo-card-content-padding-block);
    padding-inline: var(--gpp-woo-card-content-padding-inline);
  }
}
</style>

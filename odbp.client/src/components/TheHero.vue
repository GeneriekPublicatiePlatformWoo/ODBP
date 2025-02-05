<template>
  <section
    class="gpp-woo-main-image"
    :style="resources?.image ? `background-image: url(${resources?.image})` : undefined"
  >
    <div class="utrecht-page utrecht-page-content">
      <div class="gpp-woo-card">
        <div class="gpp-woo-card-content">
          <span class="utrecht-heading-2"
            >Open {{ naam }}, zoek hier in openbaar gemaakte informatie de van
            Gemeente {{ naam }}</span
          >

          <utrecht-paragraph>
            <router-link
              :to="{ name: 'home' }"
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
import { computed, inject } from "vue";
import type { Resources } from "@/resources";

const resources = inject<Resources>("resources");
const naam = computed(() => resources?.name ? resources.name : "-");
</script>

<style lang="scss" scoped>
.gpp-woo-main-image {
  background-position: 50%;
  background-repeat: no-repeat;
  background-size: cover;
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

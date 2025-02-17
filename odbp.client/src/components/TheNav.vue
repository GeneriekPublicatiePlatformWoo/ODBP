<template>
  <nav class="utrecht-nav-bar" aria-label="Hoofdmenu">
    <div class="utrecht-nav-bar__content">
      <ul role="list" class="utrecht-nav-list" id="menu">
        <li class="utrecht-nav-list__item">
          <router-link
            :to="{ name: 'home' }"
            class="utrecht-link utrecht-link--html-a utrecht-nav-list__link"
            >Home</router-link
          >
        </li>

        <li class="utrecht-nav-list__item">
          <router-link
            :to="{ name: 'zoeken' }"
            class="utrecht-link utrecht-link--html-a utrecht-nav-list__link"
            >Zoeken</router-link
          >
        </li>

        <li v-if="resources?.websiteUrl" class="utrecht-nav-list__item">
          <utrecht-link
            external
            :href="resources.websiteUrl"
            class="utrecht-nav-list__link gpp-woo-link--icon"
          >
            Naar de gemeente

            <span class="visually-hidden">(externe link)</span>

            <gpp-woo-icon icon="external" />
          </utrecht-link>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { injectResources } from "@/resources";
import GppWooIcon from "@/components/GppWooIcon.vue";

const resources = injectResources();
</script>

<style lang="scss" scoped>
.gpp-woo-link--icon {
  --utrecht-link-icon-size: 0.75rem;
}
</style>

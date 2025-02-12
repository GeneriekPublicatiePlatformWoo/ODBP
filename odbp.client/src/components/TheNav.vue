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

        <li v-if="resources?.website" class="utrecht-nav-list__item">
          <utrecht-link external :href="resources.website" class="utrecht-nav-list__link"
            >Naar de gemeente</utrecht-link
          >
        </li>
      </ul>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { injectResources } from "@/resources";

const resources = injectResources();
</script>

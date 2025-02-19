<template>
  <span class="utrecht-icon" role="presentation" aria-hidden="true">
    <svg fill="currentColor">
      <use :xlink:href="`${sprite}#${icon}`"></use>
    </svg>
  </span>
</template>

<script setup lang="ts">
import sprite from "@/assets/icon-sprite.svg";
defineProps<{ icon: string }>();
</script>

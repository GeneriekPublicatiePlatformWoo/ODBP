<template>
  <span class="utrecht-icon" role="presentation" aria-hidden="true">
    <svg fill="currentColor">
      <use :xlink:href="`/icon-sprite.svg#${icon}`"></use>
    </svg>
  </span>
</template>

<script setup lang="ts">
defineProps<{ icon: string }>();
</script>

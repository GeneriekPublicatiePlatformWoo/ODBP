<template>
  <span class="utrecht-icon" role="presentation" aria-hidden="true">
    <svg viewBox="0 0 24 24" fill="currentColor">
      <use :xlink:href="`/icons.svg#${icon}`"></use>
    </svg>
  </span>
</template>

<script setup lang="ts">
defineProps<{ icon: string }>();
</script>

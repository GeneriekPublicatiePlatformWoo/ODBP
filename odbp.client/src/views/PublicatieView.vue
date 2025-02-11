<template>
  <publicatie-details :uuid="uuid" />
</template>

<script setup lang="ts">
import PublicatieDetails from "@/features/publicatie/PublicatieDetails.vue";

defineProps<{ uuid: string }>();
</script>

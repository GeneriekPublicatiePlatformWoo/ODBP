<template>
  <document-details :uuid="uuid" />
</template>

<script setup lang="ts">
import DocumentDetails from "@/features/publicatie/DocumentDetails.vue";

defineProps<{ uuid: string }>();
</script>

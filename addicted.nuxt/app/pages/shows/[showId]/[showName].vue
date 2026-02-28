<script setup lang="ts">
import MediaDetailView from "~/components/media/MediaDetailView.vue";
import { usePageLayout } from "~/composables/usePageLayout";

const route = useRoute();
const layout = usePageLayout();
const showId = route.params.showId.toString();

definePageMeta({
  name: "show-details",
})
</script>

<template>
  <v-container fluid :class="layout.classes.pageContainer" :style="{ maxWidth: layout.maxWidth }">
    <media-detail-view :show-id="showId" />
  </v-container>
</template>

<style scoped></style>

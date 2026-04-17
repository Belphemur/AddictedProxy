<script setup lang="ts">
import MediaDetailView from "~/components/media/MediaDetailView.vue";
import { usePageLayout } from "~/composables/usePageLayout";

definePageMeta({
    name: "show-season",
    validate(route) {
        const s = route.params.seasonNumber;
        return typeof s === "string" && /^\d+$/.test(s) && parseInt(s, 10) > 0;
    },
});

const route = useRoute();
const layout = usePageLayout();
const showId = route.params.showId as string;
const initialSeason = parseInt(route.params.seasonNumber as string, 10);
</script>

<template>
    <v-container fluid :class="layout.classes.pageContainer" :style="{ maxWidth: layout.maxWidth }">
        <media-detail-view :show-id="showId" :initial-season="initialSeason" />
    </v-container>
</template>

<style scoped></style>

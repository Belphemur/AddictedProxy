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
const getSingleRouteParam = (value: string | string[]) => Array.isArray(value) ? value[0] ?? "" : value;
const showId = getSingleRouteParam(route.params.showId);
const initialSeason = parseInt(getSingleRouteParam(route.params.seasonNumber), 10);

useSeoMeta({
    robots: "noindex,follow",
});
</script>

<template>
    <v-container fluid :class="layout.classes.pageContainer" :style="{ maxWidth: layout.maxWidth }">
        <media-detail-view :show-id="showId" :initial-season="initialSeason" />
    </v-container>
</template>

<style scoped></style>

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
const runtimeConfig = useRuntimeConfig();
const requestUrl = useRequestURL();
const layout = usePageLayout();
const showId = route.params.showId as string;
const showName = route.params.showName as string;
const initialSeason = parseInt(route.params.seasonNumber as string, 10);
const siteOrigin = runtimeConfig.public.url || requestUrl.origin;

useSeoMeta({
    robots: "noindex,follow",
});

useHead({
    link: [
        {
            rel: "canonical",
            href: new URL(
                `/shows/${encodeURIComponent(showId)}/${encodeURIComponent(showName)}`,
                siteOrigin,
            ).href,
        },
    ],
});
</script>

<template>
    <v-container fluid :class="layout.classes.pageContainer" :style="{ maxWidth: layout.maxWidth }">
        <media-detail-view :show-id="showId" :initial-season="initialSeason" />
    </v-container>
</template>

<style scoped></style>

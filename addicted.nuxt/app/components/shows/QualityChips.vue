<template>
    <span v-if="!qualities?.length" class="text-medium-emphasis text-caption">—</span>
    <v-chip-group v-else column>
        <v-chip v-for="q in qualities" :key="q" :color="qualityColor(q)" size="x-small" label class="font-weight-bold">
            {{ qualityLabel(q) }}
        </v-chip>
    </v-chip-group>
</template>

<script setup lang="ts">
import { VideoQuality } from "~/composables/api/data-contracts";

interface Props {
    qualities?: VideoQuality[] | null;
}

const props = defineProps<Props>();

const labelMap: Record<VideoQuality, string> = {
    [VideoQuality.None]: "—",
    [VideoQuality.Q360P]: "360P",
    [VideoQuality.Q480P]: "480P",
    [VideoQuality.Q720P]: "720P",
    [VideoQuality.Q1080P]: "1080P",
    [VideoQuality.Q2160P]: "4K",
};

const colorMap: Record<VideoQuality, string> = {
    [VideoQuality.None]: "grey",
    [VideoQuality.Q360P]: "grey-darken-1",
    [VideoQuality.Q480P]: "grey",
    [VideoQuality.Q720P]: "light-blue",
    [VideoQuality.Q1080P]: "indigo",
    [VideoQuality.Q2160P]: "deep-purple",
};

const qualityLabel = (q: VideoQuality) => labelMap[q] ?? q;
const qualityColor = (q: VideoQuality) => colorMap[q] ?? "grey";
</script>

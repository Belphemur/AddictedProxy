<template>
    <span v-if="!qualities?.length" class="text-medium-emphasis text-caption">—</span>
    <span v-else class="d-inline-flex ga-1 flex-wrap">
        <v-chip v-for="q in qualities" :key="q" :color="qualityColor(q)" size="x-small" label
            class="font-weight-bold" variant="flat" readonly>
            {{ q }}
        </v-chip>
    </span>
</template>

<script setup lang="ts">
interface Props {
    qualities?: string[] | null;
}

defineProps<Props>();

const colorMap: Record<string, string> = {
    "360p": "grey-darken-1",
    "480p": "grey",
    "720p": "light-blue",
    "1080p": "indigo",
    "4K": "deep-purple",
};

const qualityColor = (q: string) => colorMap[q] ?? "grey";
</script>

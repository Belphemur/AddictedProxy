<template>
    <div v-if="seasonPacks.length > 0">
        <v-data-table v-if="!device.isMobile" :items="seasonPacks" :headers="headers" :items-per-page="-1"
            hide-default-footer class="transparent-table">
            <template v-slot:item.releaseGroups="{ item }">
                <div class="d-flex flex-wrap ga-1">
                    <v-chip v-for="group in item.releaseGroups" :key="group" size="small" label>{{ group }}</v-chip>
                </div>
            </template>
            <template v-slot:item.qualities="{ item }">
                <shows-quality-chips :qualities="item.qualities" />
            </template>
            <template v-slot:item.source="{ item }">
                <v-chip :color="item.source === 'SuperSubtitles' ? 'teal' : 'blue-darken-2'" size="small"
                    label>{{ item.source }}</v-chip>
            </template>
            <template v-slot:item.downloadCount="{ item }">
                <v-btn color="primary" :prepend-icon="mdiDownload" @click="downloadSeasonPack(item)"
                    :disabled="currentlyDownloading.has(item.subtitleId)">
                    {{ item.downloadCount }}
                </v-btn>
                <v-progress-linear v-show="currentlyDownloading.has(item.subtitleId)" :value="100" color="success"
                    indeterminate></v-progress-linear>
            </template>
        </v-data-table>
        <v-expansion-panels v-else bg-color="transparent">
            <v-expansion-panel v-for="pack in seasonPacks" :key="pack.subtitleId"
                :bg-color="layout.colors.expansionPanel">
                <v-expansion-panel-title>
                    <div class="d-flex align-center ga-2">
                        <v-icon size="small">{{ mdiPackageVariantClosed }}</v-icon>
                        <span class="font-weight-medium"
                            style="min-width:0; word-break:break-word;">{{ pack.releaseGroups.join(', ') || pack.version }}</span>
                    </div>
                </v-expansion-panel-title>
                <v-expansion-panel-text>
                    <v-sheet rounded="lg" :color="layout.colors.nestedItem" :class="layout.classes.nestedItem">
                        <div class="d-flex align-center ga-2 mb-2">
                            <div v-if="pack.releaseGroups.length" class="d-flex flex-wrap ga-1">
                                <v-chip v-for="group in pack.releaseGroups" :key="group" size="small" label>{{ group }}</v-chip>
                            </div>
                            <v-chip :color="pack.source === 'SuperSubtitles' ? 'teal' : 'blue-darken-2'" size="small"
                                label>{{ pack.source }}</v-chip>
                        </div>
                        <div v-if="pack.qualities?.length" class="d-flex align-center flex-wrap ga-2 mb-2">
                            <shows-quality-chips :qualities="pack.qualities" />
                        </div>
                        <div v-if="pack.uploader" class="text-body-2 text-medium-emphasis mb-2">
                            Uploaded by {{ pack.uploader }}
                        </div>
                        <v-btn color="primary" size="default" :prepend-icon="mdiDownload"
                            @click="downloadSeasonPack(pack)" :disabled="currentlyDownloading.has(pack.subtitleId)"
                            :loading="currentlyDownloading.has(pack.subtitleId)" block class="mt-1">
                            Download ZIP
                        </v-btn>
                    </v-sheet>
                </v-expansion-panel-text>
            </v-expansion-panel>
        </v-expansion-panels>
    </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import type { SeasonPackSubtitleDto } from "~/composables/api/data-contracts";
import { useSubtitles } from "~/composables/rest/api";
import { trim } from "~/composables/utils/trim";
import { mdiDownload, mdiPackageVariantClosed } from "@mdi/js";
import { usePageLayout } from "~/composables/usePageLayout";

interface Props {
    seasonPacks: SeasonPackSubtitleDto[];
}

const props = defineProps<Props>();
const layout = usePageLayout();
const subtitlesApi = useSubtitles();
const device = useDevice();

const headers = [
    { title: "Release Group", key: "releaseGroups" },
    { title: "Quality", key: "qualities" },
    { title: "Uploader", key: "uploader" },
    { title: "Source", key: "source" },
    { title: "Downloads", key: "downloadCount" },
];

const currentlyDownloading = ref<Map<string, boolean>>(new Map());

const downloadSeasonPack = async (pack: SeasonPackSubtitleDto) => {
    currentlyDownloading.value.set(pack.subtitleId, true);

    try {
        const response = await subtitlesApi.downloadSubtitle(pack.subtitleId);
        const header = response.headers.get("Content-Disposition");
        const parts = header!.split(";");
        const filename = parts[1].split("=")[1] ?? "season-pack.zip";

        const link = document.createElement("a");
        link.rel = "noopener nofollow noreferrer";
        link.href = URL.createObjectURL(await response.blob());
        link.download = trim(filename, '"');
        link.click();
        URL.revokeObjectURL(link.href);

        pack.downloadCount++;
    } finally {
        currentlyDownloading.value.delete(pack.subtitleId);
    }
};
</script>

<style scoped>
.transparent-table {
    background: transparent !important;
}

.transparent-table :deep(.v-table__wrapper) {
    background: transparent;
}
</style>

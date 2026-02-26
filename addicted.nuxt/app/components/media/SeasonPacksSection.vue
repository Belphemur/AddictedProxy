<template>
    <div v-if="seasonPacks.length > 0">
        <v-data-table v-if="!device.isMobile" :items="seasonPacks" :headers="headers" :items-per-page="-1"
            hide-default-footer class="transparent-table">
            <template v-slot:item.releaseGroups="{ item }">
                <shows-release-group-chips :groups="item.releaseGroups" />
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
                    {{ localDownloadCounts.get(item.subtitleId) ?? item.downloadCount }}
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
                            <shows-release-group-chips v-if="pack.releaseGroups.length" :groups="pack.releaseGroups" />
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
import { mevent } from "~/composables/data/event";

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

const currentlyDownloading = ref<Set<string>>(new Set());
const localDownloadCounts = ref<Map<string, number>>(new Map());

const RFC5987_PREFIX = "utf-8''";

const downloadSeasonPack = async (pack: SeasonPackSubtitleDto) => {
    mevent("download-subtitle", { subtitleId: pack.subtitleId, seasonPack: true });
    currentlyDownloading.value.add(pack.subtitleId);

    try {
        const response = await subtitlesApi.downloadSubtitle(pack.subtitleId);

        if (!response.ok) {
            console.error("Failed to download season pack subtitle", {
                subtitleId: pack.subtitleId,
                status: response.status,
                statusText: response.statusText,
            });
        } else {
            const header = response.headers.get("Content-Disposition");
            let filename = "season-pack.zip";

            if (header) {
                const parts = header.split(";").map(p => p.trim());
                const filenameStarPart = parts.find(p => p.toLowerCase().startsWith("filename*="));
                if (filenameStarPart) {
                    const value = filenameStarPart.substring(filenameStarPart.indexOf("=") + 1).trim();
                    const withoutQuotes = trim(value, '"');
                    const encoded = withoutQuotes.toLowerCase().startsWith(RFC5987_PREFIX)
                        ? withoutQuotes.substring(RFC5987_PREFIX.length)
                        : withoutQuotes;
                    try {
                        filename = decodeURIComponent(encoded);
                    } catch {
                        filename = withoutQuotes || filename;
                    }
                } else {
                    const filenamePart = parts.find(p => p.toLowerCase().startsWith("filename="));
                    if (filenamePart) {
                        const value = filenamePart.substring(filenamePart.indexOf("=") + 1).trim();
                        filename = trim(value, '"') || filename;
                    }
                }
            }

            const link = document.createElement("a");
            link.rel = "noopener nofollow noreferrer";
            link.href = URL.createObjectURL(await response.blob());
            link.download = filename;
            link.click();
            URL.revokeObjectURL(link.href);

            const currentCount = localDownloadCounts.value.get(pack.subtitleId) ?? pack.downloadCount;
            localDownloadCounts.value.set(pack.subtitleId, currentCount + 1);
        }
    } finally {
        currentlyDownloading.value.delete(pack.subtitleId);
    }
};
</script>

<style scoped>
.transparent-table {
    background: transparent;
}

.transparent-table :deep(.v-table__wrapper) {
    background: transparent;
}
</style>

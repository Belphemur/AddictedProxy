<template>
  <div>
    <v-sheet v-if="noSubtitles" rounded="lg" color="error" class="text-center pa-4">
      <v-icon size="50" class="mb-2">{{ mdiAlertCircle }}</v-icon>
      <div>Sorry, we couldn't find subtitles for your language for this season.</div>
    </v-sheet>
    <template v-else>
      <v-data-table :items="subtitles" :headers="headers" :items-per-page="-1" hide-default-footer :group-by="groupBy"
        v-if="!device.isMobile" class="transparent-table">
        <template v-slot:group-header="{ item, columns, toggleGroup, isGroupOpen }">
          <tr>
            <td :colspan="columns.length" @click="toggleGroup(item)">
              <v-btn size="large" density="comfortable" variant="text"
                :prepend-icon="isGroupOpen(item) ? '$expand' : '$next'"> {{ item.value }}
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:item.subtitle.completed="{ item }">
          <v-icon height="24px" v-if="item.subtitle.completed">{{ mdiCheck }}</v-icon>
          <span v-else></span>
        </template>
        <template v-slot:item.subtitle.hearingImpaired="{ item }">
          <v-icon v-if="item.subtitle.hearingImpaired">{{ mdiEarHearingOff }}</v-icon>
          <span v-else></span>
        </template>
        <template v-slot:item.subtitle.qualities="{ item }">
          <shows-quality-chips :qualities="item.subtitle.qualities" />
        </template>

        <template v-slot:item.subtitle.source="{ item }">
          <v-chip :color="item.subtitle.source === 'SuperSubtitles' ? 'teal' : 'blue-darken-2'" size="small"
            label>{{ item.subtitle.source }}</v-chip>
        </template>

        <template v-slot:item.subtitle.downloadCount="{ item }">
          <v-btn color="primary" :prepend-icon="mdiDownload" @click="downloadSubtitle(item.subtitle)"
            :disabled="currentlyDownloading.has(item.subtitle.subtitleId)">
            {{ item.subtitle.downloadCount }}
          </v-btn>
          <v-progress-linear v-show="currentlyDownloading.has(item.subtitle.subtitleId)" :value="100" color="success"
            indeterminate></v-progress-linear>
        </template>

      </v-data-table>
      <v-expansion-panels v-else bg-color="transparent">
        <v-expansion-panel v-for="episode in props.episodes" bg-color="rgba(255,255,255,0.05)">
          <v-expansion-panel-title>
            <h3> Ep {{ episode.number }}. {{ episode.title }}</h3>
          </v-expansion-panel-title>
          <v-expansion-panel-text>
            <div v-for="subtitle in episode.subtitles" :key="subtitle.subtitleId" class="mb-3">
              <v-sheet rounded="lg" color="rgba(255,255,255,0.08)" class="pa-3">
                <div class="d-flex align-center ga-2 mb-2">
                  <v-icon size="small" class="flex-shrink-0">{{ mdiSubtitlesOutline }}</v-icon>
                  <span class="font-weight-medium"
                    style="min-width:0; word-break:break-word;">{{ subtitle.version }}</span>
                  <div v-if="subtitle.hearingImpaired" class="d-flex align-center ga-1 flex-shrink-0 ms-auto">
                    <v-icon size="small">{{ mdiEarHearingOff }}</v-icon>
                    <span class="text-body-2">HI</span>
                  </div>
                </div>
                <div class="d-flex align-center flex-wrap ga-2 mb-2">
                  <shows-quality-chips v-if="subtitle.qualities?.length" :qualities="subtitle.qualities" />
                  <v-chip :color="subtitle.source === 'SuperSubtitles' ? 'teal' : 'blue-darken-2'" size="small"
                    label>{{ subtitle.source }}</v-chip>
                </div>
                <v-btn color="primary" size="default" :prepend-icon="mdiDownload" @click="downloadSubtitle(subtitle)"
                  :disabled="currentlyDownloading.has(subtitle.subtitleId)"
                  :loading="currentlyDownloading.has(subtitle.subtitleId)" block class="mt-1">
                  Download
                </v-btn>
              </v-sheet>
            </div>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
    </template>
  </div>
</template>

<script setup lang="ts">
import { defineProps, ref } from "vue";

import { mevent } from "~/composables/data/event";
import type { EpisodeWithSubtitlesDto, SubtitleDto } from "~/composables/api/data-contracts";
import { useSubtitles } from "~/composables/rest/api";
import type { SubtitleWithEpisode } from "~/composables/dto/SubtitleWithEpisode";
import { forEach, orderBy } from "lodash-es";
import { trim } from "~/composables/utils/trim";
import { mdiAlertCircle, mdiCheck, mdiDownload, mdiEarHearingOff, mdiSubtitlesOutline } from "@mdi/js";

interface Props {
  episodes: Array<EpisodeWithSubtitlesDto> | null;
}

const subtitlesApi = useSubtitles();

const props = defineProps<Props>();

const device = useDevice();

const groupBy = [
  {
    key: 'title'
  }
]
const headers = [
  { title: "Version", key: "subtitle.version" },
  { title: "Completed", key: "subtitle.completed" },
  { title: "Hearing Impaired", key: "subtitle.hearingImpaired" },
  { title: "Quality", key: "subtitle.qualities" },
  { title: "Source", key: "subtitle.source" },
  { title: "Downloads", key: "subtitle.downloadCount" },
];

const noSubtitles = computed<boolean>(() => {
  if (props.episodes == null) return true;
  return props.episodes!.every((e) => e.subtitles?.length === 0)
});

const subtitles = computed<SubtitleWithEpisode[]>(() => {
  if (props.episodes == null) return [];

  const subtitles: SubtitleWithEpisode[] = [];
  forEach(props.episodes, (episode: EpisodeWithSubtitlesDto) => {
    forEach(orderBy(episode.subtitles, (subtitle) => subtitle.hearingImpaired), (subtitle: SubtitleDto) => {
      subtitles.push({
        subtitle: subtitle,
        episode: episode,
        title: `${episode.number} — ${episode.title}`
      });
    })
  })
  return subtitles;
});

const currentlyDownloading = ref<Map<string, boolean>>(new Map());
const downloadSubtitle = async (sub: SubtitleDto) => {
  mevent("download-subtitle", { subtitle: sub });
  currentlyDownloading.value.set(sub.subtitleId!, true);
  const UpdateDownloaded = () => {
    sub.downloadCount!++;
    currentlyDownloading.value.delete(sub.subtitleId!);
  };

  const response = await subtitlesApi.downloadSubtitle(sub.subtitleId!);
  const header = response.headers.get("Content-Disposition");
  const parts = header!.split(";");
  const filename = parts[1].split("=")[1] ?? "sub.srt";

  const link = document.createElement("a");
  link.rel = "noopener nofollow noreferrer";
  link.href = URL.createObjectURL(await response.blob());
  link.download = trim(filename, '"')
  link.click();
  URL.revokeObjectURL(link.href);

  UpdateDownloaded();
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

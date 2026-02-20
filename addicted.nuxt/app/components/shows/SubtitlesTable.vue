<template>
  <v-container>
    <v-sheet v-if="noSubtitles" rounded="xl" color="error" class="text-center">
      <v-row>
        <v-col>
          <v-icon size="50">{{ mdiAlertCircle }}</v-icon>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          Sorry, we couldn't find subtitles for your language for this season.
        </v-col>
      </v-row>
    </v-sheet>
    <v-container v-else>
      <v-data-table :items="subtitles" :headers="headers" :items-per-page="-1" hide-default-footer :group-by="groupBy"
        v-if="!device.isMobile">
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
          <quality-chips :qualities="item.subtitle.qualities" />
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
      <v-expansion-panels v-else bg-color="#2f3237">
        <v-expansion-panel v-for="episode in props.episodes">
          <v-expansion-panel-title>
            <h3> Ep {{ episode.number }}. {{ episode.title }}</h3>
          </v-expansion-panel-title>
          <v-expansion-panel-text>
            <v-row v-for="subtitle in episode.subtitles" :key="subtitle.subtitleId" class="mb-4">
              <v-col>
                <v-card elevation="2" outlined shaped tile :loading="currentlyDownloading.has(subtitle.subtitleId)">
                  <v-card-title>
                    <h3>{{ subtitle.title }}</h3>
                  </v-card-title>
                  <v-card-text>
                    <v-row>
                      <v-col cols="12">
                        <v-icon>{{ mdiSubtitlesOutline }}</v-icon>
                        {{ subtitle.version }}
                      </v-col>
                      <v-col v-if="subtitle.completed" cols="12">
                        <v-icon>{{ mdiCheck }}</v-icon>
                        Completed
                      </v-col>
                      <v-col v-if="subtitle.hearingImpaired" cols="12">
                        <v-icon>{{ mdiEarHearingOff }}</v-icon>
                        Hearing Impaired
                      </v-col>
                      <v-col v-if="subtitle.qualities?.length" cols="12">
                        <quality-chips :qualities="subtitle.qualities" />
                      </v-col>
                      <v-col cols="12">
                        <v-chip :color="subtitle.source === 'SuperSubtitles' ? 'teal' : 'blue-darken-2'" size="small"
                          label>{{ subtitle.source }}</v-chip>
                      </v-col>
                    </v-row>
                  </v-card-text>
                  <v-card-actions class="justify-center">
                    <v-btn color="primary" :prepend-icon="mdiDownload" @click="downloadSubtitle(subtitle)"
                      :disabled="currentlyDownloading.has(subtitle.subtitleId)">
                      Download subtitle
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-container>
  </v-container>
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
<style scoped></style>

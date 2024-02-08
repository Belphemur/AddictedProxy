<template>
  <v-container>
    <v-sheet v-if="noSubtitles" rounded="xl" color="error" class="text-center">
      <v-row>
        <v-col>
          <v-icon icon="mdi-alert-circle" size="50"></v-icon>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          Sorry, we couldn't find subtitles for your language for this season.
        </v-col>
      </v-row>
    </v-sheet>
    <v-container v-else>
      <v-data-table
          :items="subtitles"
          :headers="headers" items-per-page="25"
          :group-by="groupBy">
        <template v-slot:group-header="{ item, columns, toggleGroup, isGroupOpen }">
          <tr>
            <td :colspan="columns.length" @click="toggleGroup(item)">
              <v-btn
                  size="large"
                  density="comfortable"
                  variant="text"
                  :prepend-icon="isGroupOpen(item) ? '$expand' : '$next'"
              > {{ item.value }}</v-btn>

            </td>
          </tr>
        </template>
        <template v-slot:item.subtitle.completed="{item}">
          <v-icon icon="mdi-check" height="24px" v-if="item.subtitle.completed"/>
          <span v-else></span>
        </template>
        <template v-slot:item.subtitle.hearingImpaired="{item}">
          <v-icon icon="mdi-ear-hearing-off" v-if=" item.subtitle.hearingImpaired"/>
          <span v-else></span>
        </template>
        <template v-slot:item.subtitle.hd="{item}">
          <v-icon icon="mdi-check" height="24px" v-if="item.subtitle.hd"/>
          <span v-else></span>
        </template>

        <template v-slot:item.subtitle.downloadCount="{item}">
          <v-btn
              color="primary"
              prepend-icon="mdi-download"
              @click="downloadSubtitle(item.subtitle)"
              :disabled="currentlyDownloading.has(item.subtitle.subtitleId)"
          >
            {{ item.subtitle.downloadCount }}
          </v-btn>
          <v-progress-linear
              v-show="currentlyDownloading.has(item.subtitle.subtitleId)"
              :value="100"
              color="success"
              indeterminate
          ></v-progress-linear>
        </template>

      </v-data-table>
    </v-container>
  </v-container>
</template>

<script setup lang="ts">
import {defineProps, ref} from "vue";

import {mevent} from "~/composables/data/event";
import type {EpisodeWithSubtitlesDto, SubtitleDto} from "~/composables/api/data-contracts";
import {useSubtitles} from "~/composables/rest/api";
import type {SubtitleWithEpisode} from "~/composables/dto/SubtitleWithEpisode";
import {forEach} from "lodash-es";

interface Props {
  episodes: Array<EpisodeWithSubtitlesDto> | null;
}

const subtitlesApi = useSubtitles();

const props = defineProps<Props>();

const groupBy = [
  {
    key: 'title'
  }
]
const headers = [
  {title: "Version", key:"subtitle.version"},
  {title: "Completed", key: "subtitle.completed"},
  {title: "Hearing Impaired", key:"subtitle.hearingImpaired"},
  {title: "HD", key:"subtitle.hd"},
  {title: "Downloads", key:"subtitle.downloadCount"},
];

const noSubtitles = computed<boolean>(() => {
  if (props.episodes == null) return true;
  return props.episodes!.every((e) => e.subtitles?.length === 0)
});

const subtitles = computed<SubtitleWithEpisode[]>(() => {
  if (props.episodes == null) return [];

  const subtitles: SubtitleWithEpisode[] = [];
  forEach(props.episodes, (episode: EpisodeWithSubtitlesDto) => {
    forEach(useOrderBy(episode.subtitles, (subtitle) => subtitle.hearingImpaired), (subtitle: SubtitleDto) => {
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
  mevent("download-subtitle", {subtitle: sub});
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
  link.href = URL.createObjectURL(await response.blob());
  link.download = filename;
  link.click();
  URL.revokeObjectURL(link.href);

  UpdateDownloaded();
};
</script>
<style scoped>
</style>

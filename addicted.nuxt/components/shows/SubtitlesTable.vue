<template>
  <v-container>
    <v-expansion-panels>
      <v-expansion-panel
        v-for="episode in props.episodes"
        v-bind:key="episode.number"
        :title="`Ep. ${episode.number}: ${episode.title}`"
      >
        <v-expansion-panel-text>
          <v-data-table v-if="episode.subtitles" :items="episode.subtitles" :headers="headers" items-per-page="25">
            <template #item.version="{ item }">
              {{ item.selectable.version }}
            </template>
            <template #item.completed="{ item }">
              <v-icon icon="mdi-check" height="24px" v-if=" item.selectable.completed"/>
              <span v-else></span>
            </template>
            <template #item.hearingImpaired="{ item }">
              <v-icon icon="mdi-ear-hearing-off" v-if=" item.selectable.hearingImpaired"/>
              <span v-else></span>
            </template>
            <template #item.hd="{ item }">
              <v-icon icon="mdi-check" height="24px" v-if=" item.selectable.hd"/>
              <span v-else></span>
            </template>
            <template #item.downloadCount="{ item }">
              <v-btn
                color="primary"
                prepend-icon="mdi-download"
                @click="downloadSubtitle(item.selectable)"
                :disabled="currentlyDownloading.has(item.selectable.subtitleId)"
              >
                {{ item.selectable.downloadCount }}
              </v-btn>
              <v-progress-linear
                v-show="currentlyDownloading.has(item.selectable.subtitleId)"
                :value="100"
                color="success"
                indeterminate
              ></v-progress-linear>
            </template>
          </v-data-table>
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>
  </v-container>
</template>

<script setup lang="ts">
import {defineProps, ref} from "vue";

import {EpisodeWithSubtitlesDto, SubtitleDto} from "@/api/api";
import {mevent} from "@/composables/matomo/event";
import {useApi} from "~/composables/rest/api";

interface Props {
  episodes: Array<EpisodeWithSubtitlesDto>;
}
const api = useApi();

const props = defineProps<Props>();

const headers = [
  {title: "Version", key: "version"},
  {title: "Completed", key: "completed"},
  {title: "Hearing Impaired", key: "hearingImpaired"},
  {title: "HD", key: "hd"},
  {title: "Downloads", key: "downloadCount"},
];

const currentlyDownloading = ref<Map<string, boolean>>(new Map());
const downloadSubtitle = async (sub: SubtitleDto) => {
  mevent("download-subtitle", {subtitle: sub});
  currentlyDownloading.value.set(sub.subtitleId!, true);
  const UpdateDownloaded = () => {
    sub.downloadCount!++;
    currentlyDownloading.value.delete(sub.subtitleId!);
  };

  const response = await api.subtitles.downloadSubtitle(sub.subtitleId!);
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

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
    <v-expansion-panels v-else>
      <v-expansion-panel
          v-for="episode in useFilter(props.episodes, (episode) => episode.subtitles?.length > 0)"
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

import {mevent} from "~/composables/data/event";
import {EpisodeWithSubtitlesDto, SubtitleDto} from "~/composables/api/data-contracts";
import {useSubtitles} from "~/composables/rest/api";

interface Props {
  episodes: Array<EpisodeWithSubtitlesDto>;
}

const subtitlesApi = useSubtitles();

const props = defineProps<Props>();

const headers = [
  {title: "Version", key: "version"},
  {title: "Completed", key: "completed"},
  {title: "Hearing Impaired", key: "hearingImpaired"},
  {title: "HD", key: "hd"},
  {title: "Downloads", key: "downloadCount"},
];

const noSubtitles = computed<boolean>(() => props.episodes.every((e) => e.subtitles?.length === 0));

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

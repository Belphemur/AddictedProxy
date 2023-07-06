<script setup lang="ts">
import {useApi} from "@/composables/rest/api";
import MediaDetails from "@/components/media/MediaDetails.vue";
import {ref, onUnmounted} from "vue";
import {EpisodeWithSubtitlesDto, MediaDetailsDto} from "~/composables/api/api";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";
import {DoneHandler, ProgressHandler, useRefreshHub} from "~/composables/hub/RefreshHub";


export interface Props {
  showId: string;
}

const props = defineProps<Props>();
const api = useApi();
const loadingEpisodes = ref(false);
const episodes = ref<EpisodeWithSubtitlesDto[]>([]);
const refreshingProgress = ref<number | null>(null);
const language = useLanguage();
const currentSeason = ref<number | undefined>(undefined);
const mediaInfo = ref<MediaDetailsDto>();

await loadMediaDetails();


useSeoMeta({
  title: `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}`,
  description: `Find all the subtitle you need for your favorite show ${mediaInfo.value!.media?.name}`,
  ogDescription: `Find all the subtitle you need for your favorite show ${mediaInfo.value!.media?.name}`,
  ogImage: mediaInfo.value!.details?.backdropPath ?? mediaInfo.value!.details?.posterPath
})

async function loadShowData() {
  loadingEpisodes.value = true;
  const response = await api.shows.showsDetail(props.showId, currentSeason.value!, language.lang)
  episodes.value = response.data.episodes!;
  loadingEpisodes.value = false;
}

const {start, sendRefreshAsync, unsubscribeShowAsync, onProgress, offProgress, onDone, offDone} = useRefreshHub();

await start();

const progressHandler: ProgressHandler = (progress) => {
  if (refreshingProgress.value != null && progress.progress < refreshingProgress.value) {
    console.error("Got progress lower than current value");
    return;
  }
  refreshingProgress.value = progress.progress;
};

const doneHandler: DoneHandler = async (show) => {
  if (show.id != props.showId) {
    return;
  }
  refreshingProgress.value = null;
  await unsubscribeShowAsync(show.id!);
  if (currentSeason.value == undefined) {
    await loadMediaDetails();
  }
  await loadShowData();
};


onProgress(progressHandler);
onDone(doneHandler);
onUnmounted(() => {
  offProgress(progressHandler);
  offDone(doneHandler);
});

async function loadMediaDetails() {
  const response = await api.media.mediaDetails(props.showId);
  mediaInfo.value = response.data
  currentSeason.value = useLast(mediaInfo.value.media?.seasons);
}


const refreshShow = async () => {
  refreshingProgress.value = 0;
  await sendRefreshAsync(props.showId);
};

if (currentSeason.value == undefined) {
  await refreshShow();
  loadingEpisodes.value = true;
} else {
  const responseEpisode = await api.shows.showsDetail(props.showId, currentSeason.value, language.lang);
  episodes.value = responseEpisode.data.episodes!;
}
const formattedProgress = computed(() => {
  let keyword = "";
  if (refreshingProgress.value == null) {
    return "";
  }
  switch (true) {
    case refreshingProgress.value == 100:
      return `Ready`;
    case refreshingProgress.value < 25:
      keyword = "Fetching seasons";
      break;
    case refreshingProgress.value >= 25:
      keyword = "Fetching subtitles";
      break;
    default:
      keyword = "Fetching";
      break;
  }
  return `${keyword}: (${refreshingProgress.value}%)`;
});

watch(currentSeason, async (value) => {
  if (value == undefined) {
    return;
  }
  await loadShowData();
})
watch(language, async () => {
  await loadShowData();
});

</script>

<template>
  <div>
    <v-row>
      <v-col cols="12">
        <media-details :details="mediaInfo!" v-model="currentSeason"/>
      </v-col>
    </v-row>
    <v-row justify="center" v-if="refreshingProgress != null">
      <v-col cols="10">
        <v-col cols="11" align-self="center">
          <v-progress-linear
              v-model="refreshingProgress"
              color="blue"
              height="18"
          >
            {{ formattedProgress }}
          </v-progress-linear>
        </v-col>
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-skeleton-loader type="card" :loading="loadingEpisodes">
        <v-card>
          <v-card-title>Season {{ currentSeason }}</v-card-title>
          <v-card-actions>
            <v-btn
                prepend-icon="mdi-refresh"
                class="text-none mb-4"
                color="indigo-lighten-3"
                @click="refreshShow"
                :disabled="refreshingProgress != null"
            >Refresh
              <v-tooltip
                  activator="parent"
                  location="end"
              >Fetch from Addic7ed
              </v-tooltip>
            </v-btn>
          </v-card-actions>
          <v-card-text>
            <subtitles-table :episodes="episodes"></subtitles-table>
          </v-card-text>
        </v-card>
      </v-skeleton-loader>
    </v-row>
  </div>
</template>

<style scoped>

</style>

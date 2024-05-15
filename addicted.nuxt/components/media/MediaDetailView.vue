<script setup lang="ts">
import MediaDetails from "@/components/media/MediaDetails.vue";
import {ref, onUnmounted} from "vue";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";
import type {DoneHandler, ProgressHandler} from "~/composables/hub/RefreshHub";
import type {EpisodeWithSubtitlesDto, MediaDetailsDto} from "~/composables/api/data-contracts";
import {useMedia, useShows} from "~/composables/rest/api";
import {useRefreshHub} from "~/composables/hub/RefreshHub";


export interface Props {
  showId: string;
}

const props = defineProps<Props>();
const mediaApi = useMedia();
const showsApi = useShows();
let loadingEpisodes = ref(false);
let episodes = ref<EpisodeWithSubtitlesDto[] | null>([]);
const refreshingProgress = ref<number | null>(null);
const language = useLanguage();
const currentSeason = ref<number | undefined>(undefined);
const mediaInfo = ref<MediaDetailsDto>();

await loadMediaDetails();

const runtimeConfig = useRuntimeConfig();
let imageUrl = mediaInfo.value!.details?.backdropPath ?? mediaInfo.value!.details?.posterPath;
let twitterUrl = imageUrl;
if (imageUrl != null) {
  imageUrl += "?width=512&format=jpeg"
  twitterUrl += "?width=250&format=jpeg"
}

useSeoMeta({
  title: `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}`,
  ogTitle: `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}`,
  description: `Find all the subtitles in multiple language like English, French, etc ... your favorite show ${mediaInfo.value!.media?.name}`,
  ogDescription: `Find all the subtitles in multiple language like English, French, etc ... your favorite show ${mediaInfo.value!.media?.name}`,
  ogImage: new URL(imageUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  articleTag: mediaInfo.value!.details?.genre ?? [],
  twitterImage: new URL(twitterUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  ogImageAlt: `Poster of ${mediaInfo.value!.media?.name}`,
  twitterImageAlt: `Poster of ${mediaInfo.value!.media?.name}`,
  ogType: "website"
})

await loadShowData();

async function loadShowData() {
  const {
    data,
    pending
  } = await useAsyncData(async () => {
    if (currentSeason.value == undefined) {
      return [];
    }
    return (await showsApi.showsDetail(props.showId, currentSeason.value!, language.lang)).data.episodes!;
  }, {
    watch: [currentSeason, language]
  });

  loadingEpisodes = pending;
  episodes = data;
}

const {
  start,
  sendRefreshAsync,
  unsubscribeShowAsync,
  onProgress,
  offProgress,
  onDone,
  offDone,
  getEpisodes
} = useRefreshHub();

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

  mediaInfo.value = {details: mediaInfo.value?.details, media: show}
  if (currentSeason.value == undefined) {
    currentSeason.value = useLast(mediaInfo.value?.media?.seasons);
  }

  loadingEpisodes.value = true;
  episodes.value = await getEpisodes(show.id!, language.lang, currentSeason.value!);
  loadingEpisodes.value = false;
};


onProgress(progressHandler);
onDone(doneHandler);
onUnmounted(() => {
  offProgress(progressHandler);
  offDone(doneHandler);
});

async function loadMediaDetails() {
  const {data, error} = await useAsyncData(async () => (await mediaApi.mediaDetails(props.showId)).data!);
  if (error.value != null) {
    throw createError({statusCode: 404, statusMessage: `Show ${props.showId} not found`});
  }
  mediaInfo.value = data.value!;
  currentSeason.value = useLast(mediaInfo.value?.media?.seasons);
}


const refreshShow = async () => {
  await start();
  refreshingProgress.value = 0;
  await sendRefreshAsync(props.showId);
};

if (currentSeason.value == undefined) {
  await refreshShow();
  loadingEpisodes.value = true;
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
</script>

<template>
  <div>
    <v-row v-if="mediaInfo?.details != null">
      <v-col cols="12" offset="0" lg="8" offset-lg="2" >
        <media-details :details="mediaInfo" v-model="currentSeason"/>
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
      <v-col cols="12">
        <v-skeleton-loader type="card" :loading="loadingEpisodes">
          <v-card width="100%">
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
      </v-col>
    </v-row>
  </div>
</template>

<style scoped>

</style>
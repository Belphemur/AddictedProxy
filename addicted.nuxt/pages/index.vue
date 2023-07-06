<template>
  <v-row justify="center">
    <v-col cols="10">
      <v-card title="Welcome to Gestdown">
        <v-card-subtitle> It acts as a proxy for the Addic7ed subtitle website. You can easily
          search here for subtitle available on the platform and download them.
        </v-card-subtitle>
        <v-card-text>
          <v-row>
            <v-col>
              <h2 class="text-h4">Trending</h2>
            </v-col>
          </v-row>
          <v-row justify-lg="center">
            <v-col lg="8" align-self="center">
              <MediaTrending :medias="mediaTrending"></MediaTrending>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <h2 class="text-h4">Search</h2>

              <SearchComponent
                  ref="searchBox"
                  v-on:selected="getSubtitles"
                  v-on:cleared="clear"
                  v-on:need-refresh="needRefresh"
              />
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
  <v-row justify="center">
    <v-col cols="10">
      <v-row
          v-for="[key, value] in refreshingShows"
          v-bind:key="key"
      >
        <v-col cols="11" align-self="center">
          <v-progress-linear
              v-model="value.progress"
              color="blue"
              height="18"
          >
            {{ formatPercentage(key)(value.progress) }}
          </v-progress-linear>
        </v-col>
        <v-col cols="1" align-self="center">
          <v-btn
              density="compact"
              color="blue"
              @click="selectShow(key)"
          >
            <v-icon icon="mdi-refresh"></v-icon>
            <v-tooltip
                v-if="value.show != null"
                activator="parent"
            >Redo the search
            </v-tooltip>
          </v-btn>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
  <v-row justify="center">
    <v-col cols="10">
      <v-card v-if="currentShow != null">
        <v-card-title>
          {{ currentShow.show.name }}: Season
          {{ currentShow.season }}
        </v-card-title>
        <v-card-actions>
          <v-row>
            <v-col cols="4">

              <v-btn
                  prepend-icon="mdi-refresh"
                  class="text-none mb-4"
                  color="indigo-lighten-3"
                  @click="refreshShow(currentShow.show)"
              >Refresh
                <v-tooltip
                    activator="parent"
                    location="end"
                >Fetch from Addic7ed
                </v-tooltip>
              </v-btn>
            </v-col>
            <v-col offset="7">
              <v-btn
                  :to="{name: 'show-details', params: {showId: currentShow.show.id, showName: currentShow.show.name}}"
                  prepend-icon="mdi-ballot">Details
                <v-tooltip activator="parent" location="end">
                  See details of the show {{ currentShow.show.name }}
                </v-tooltip>
              </v-btn>
            </v-col>
          </v-row>
        </v-card-actions>
        <v-card-item>
          <v-skeleton-loader :loading="loadingSubtitles" type="card">
            <subtitles-table
                v-if="episodesWithSubtitles.length > 0"
                :episodes="episodesWithSubtitles"
                style="display: flex; flex-grow: 1"
            ></subtitles-table>
          </v-skeleton-loader>
        </v-card-item>
      </v-card>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">

import {onUnmounted, ref} from "vue";
import {SelectedShow} from "@/composables/dto/SelectedShow";
import {EpisodeWithSubtitlesDto, MediaDetailsDto, ShowDto} from "~/composables/api/api";
import {
  DoneHandler,
  ProgressHandler,
  useRefreshHub
} from "@/composables/hub/RefreshHub";
import SearchComponent from "@/components/shows/SearchComponent.vue";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";
import {useApi} from "~/composables/rest/api";

interface ProgressShow {
  name: string;
  progress: number;
  show?: ShowDto;
}

definePageMeta({
  name: "Home",
  order: 10,
  icon: "mdi-search-web",
})
useSeoMeta({
  title: "Gestdown: Addic7ed Proxy",
  description: "Help you search for subtitle for different show available on Addic7ed",
  ogDescription: "Help you search for subtitle for different show available on Addic7ed",
  ogImage: "/img/logo.png"
})

const api = useApi();
const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto>>([]);

const searchBox = ref<InstanceType<typeof SearchComponent> | null>(null);

const loadingSubtitles = ref(false);
const refreshingShows = ref(new Map<string, ProgressShow>());
const currentShow = ref<SelectedShow | null>(null);
const mediaTrending = ref<Array<MediaDetailsDto>>([]);
const router = useRouter();


const {start, sendRefreshAsync, unsubscribeShowAsync, onProgress, offProgress, onDone, offDone} = useRefreshHub();

await start();

const mediaTrendingResponse = await api.media.trendingDetail(10)

mediaTrending.value = mediaTrendingResponse.data;

const selectShow = (showId: string) => {
  const showProgress = refreshingShows.value.get(showId);
  if (showProgress?.show == null) {
    return;
  }
  searchBox.value?.setSelectedShow(showProgress.show);
  refreshingShows.value.delete(showId);
};

const getSubtitles = async (show: SelectedShow) => {
  await router.push({name: 'show-details', params: {showId: show.show.id, showName: show.show.name}})
  // loadingSubtitles.value = true;
  // currentShow.value = show;
  // const response = await api.shows.showsDetail(
  //     show.show.id,
  //     show.season,
  //     show.language
  // );
  // episodesWithSubtitles.value = response.data.episodes || [];
  // loadingSubtitles.value = false;
};

const clear = () => {
  episodesWithSubtitles.value = [];
  currentShow.value = null;
};
const formatPercentage = (showId: string) => {
  const showProgress = refreshingShows.value.get(showId)!;

  return (progress: number) => {
    let keyword = "";
    switch (true) {
      case progress == 100:
        return `${showProgress.name}: Ready`;
      case progress < 25:
        keyword = "Fetching seasons of";
        break;
      case progress >= 25:
        keyword = "Fetching subtitles of";
        break;
      default:
        keyword = "Fetching";
        break;
    }
    return `${keyword} ${showProgress.name}: (${progress}%)`;
  };
};

const refreshShow = async (show: ShowDto) => {
  refreshingShows.value.set(show.id, {name: show.name, progress: 0});
  await sendRefreshAsync(show.id);
};

const needRefresh = async (show: ShowDto) => {
  // ElMessage({
  //   message:
  //     "We don't have any subtitle for that show. We're fetching them from Addic7ed.\nPlease Try later.",
  //   type: "warning",
  //   duration: 5000,
  // });
  await refreshShow(show);
};
const progressHandler: ProgressHandler = (progress) => {
  const progressShow = refreshingShows.value.get(progress.showId)!;
  if (progress.progress < progressShow.progress) {
    console.error("Got progress lower than current value");
    return;
  }
  refreshingShows.value.set(progress.showId, {
    ...progressShow,
    progress: progress.progress,
  });
};

const doneHandler: DoneHandler = async (show) => {
  await unsubscribeShowAsync(show.id!);
  const progressShow = refreshingShows.value.get(show.id)!;
  refreshingShows.value.set(show.id, {
    ...progressShow,
    show: show,
  });
  //Auto refresh if the current show is the one that has finished refreshing
  if (currentShow.value?.show.id === show.id) {
    refreshingShows.value.delete(show.id);
    await getSubtitles(currentShow.value!);
  }
};
onProgress(progressHandler);
onDone(doneHandler);
onUnmounted(() => {
  offProgress(progressHandler);
  offDone(doneHandler);
});
</script>

<style scoped>
</style>

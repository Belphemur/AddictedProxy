<template>
  <v-container fluid class="pa-4 pa-sm-6" style="max-width: 1600px">
    <v-sheet rounded="lg" color="rgba(0,0,0,0.75)" class="pa-4 pa-sm-6 mb-4">
      <div class="text-center mb-4">
        <h1 class="text-h5 text-sm-h4 font-weight-bold">
          <v-icon :icon="logo" class="mr-1" />
          Gestdown
        </h1>
        <p class="text-body-2 text-sm-body-1 text-medium-emphasis mt-1">
          Search and download subtitles from <strong>Addic7ed</strong> &amp; <strong>SuperSubtitles</strong>
        </p>
      </div>

      <v-row justify="center">
        <v-col cols="12" sm="8" md="6">
          <SearchComponent ref="searchBox" v-on:selected="goToPage" v-on:cleared="clear" />
        </v-col>
      </v-row>
    </v-sheet>

    <h2 class="text-h6 text-sm-h5 mb-2 trending-title">Trending</h2>
    <media-trending :medias="trendingMedias"></media-trending>
  </v-container>
</template>

<script setup lang="ts">

import { ref } from "vue";
import SearchComponent from "@/components/shows/SearchComponent.vue";
import logo from "@/components/icon/logo.vue";
import type { EpisodeWithSubtitlesDto, ShowDto } from "~/composables/api/data-contracts";
import { useMedia } from "~/composables/rest/api";
import type { SelectedShow } from "~/composables/dto/SelectedShow";
import { mdiSearchWeb } from "@mdi/js";

definePageMeta({
  name: "Home",
  order: 10,
  icon: mdiSearchWeb,
})
useSeoMeta({
  title: "Gestdown: Subtitle Aggregator",
  description: "Search and download subtitles from Addic7ed and SuperSubtitles in one place",
  ogDescription: "Search and download subtitles from Addic7ed and SuperSubtitles in one place",
  ogImage: new URL("/img/Gestdown-logos.jpeg", useRuntimeConfig().public.url).href,
  ogLocale: "en-US",
  ogTitle: "Gestdown: Subtitle Aggregator",
  ogSiteName: "Gestdown: Subtitle Aggregator — Addic7ed & SuperSubtitles",
  keywords: "subtitles, Addic7ed, SuperSubtitles, subtitle aggregator, download subtitles",
})

const mediaApi = useMedia();
const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto>>([]);
const searchBox = ref<InstanceType<typeof SearchComponent> | null>(null);
const currentShow = ref<SelectedShow | undefined>(undefined);


const { data, error } = await useAsyncData(async () => {
  const isMobile = useDevice().isMobile;
  return (await mediaApi.trendingDetail(isMobile ? 4 : 12)).data;
});

if (error.value != null) {
  console.error("can't get media trending", error.value);
  throw createError("Couldn't fetch trending medias");
}

const trendingMedias = data;

const goToPage = async (show: ShowDto) => {
  const router = useRouter();
  await router.push({ name: 'show-details', params: { showId: show.id, showName: show.slug } })
};

const clear = () => {
  episodesWithSubtitles.value = [];
  currentShow.value = undefined;
};

</script>

<style scoped>
.trending-title {
  text-shadow: 1px 1px 4px rgba(0, 0, 0, 0.8);
}
</style>

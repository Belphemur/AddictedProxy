<template>
  <v-row justify="center">
    <v-col cols="12" sm="10" xl="8" style="max-width: 1600px">
      <v-card>
        <v-card-item class="py-2">
          <v-card-title class="text-h6 text-sm-h5">Welcome to Gestdown</v-card-title>
          <v-card-subtitle class="text-body-2 text-sm-body-1 text-wrap">
            A subtitle aggregator pulling from multiple providers including <strong>Addic7ed</strong> and
            <strong>SuperSubtitles</strong>.
            Search and download subtitles for your favourite shows.
          </v-card-subtitle>
        </v-card-item>
        <v-card-text class="pt-1">
          <v-row dense>
            <v-col>
              <h2 class="text-h6 text-sm-h4 mb-2">Search</h2>
              <SearchComponent ref="searchBox" v-on:selected="goToPage" v-on:cleared="clear" />
            </v-col>
          </v-row>
          <v-row dense class="mt-n2">
            <v-col>
              <h2 class="text-h6 text-sm-h4">Trending</h2>
            </v-col>
          </v-row>
          <v-row dense justify-lg="center">
            <v-col align-self="center">
              <media-trending :medias="trendingMedias"></media-trending>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>

</template>

<script setup lang="ts">

import { ref } from "vue";
import SearchComponent from "@/components/shows/SearchComponent.vue";
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

<style scoped></style>

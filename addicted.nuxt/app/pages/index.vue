<template>
  <v-container fluid :class="layout.classes.pageContainer" :style="{ maxWidth: layout.maxWidth }">
    <v-sheet rounded="lg" :color="layout.colors.primaryPanel" :class="layout.classes.primaryPanel">
      <div :class="layout.classes.pageHeading">
        <h1 :class="layout.classes.pageTitle">
          <v-icon :icon="logo" class="mr-1" />
          Gestdown
        </h1>
        <p :class="layout.classes.pageSubtitle">
          Search and download subtitles from <strong>Addic7ed</strong> &amp; <strong>SuperSubtitles</strong>
        </p>
      </div>

      <v-row justify="center">
        <v-col cols="12" sm="8" md="6">
          <SearchComponent ref="searchBox" v-on:selected="goToPage" />
        </v-col>
      </v-row>
    </v-sheet>

    <v-row justify="center" class="my-3">
      <v-col cols="12" md="10" lg="8">
        <ScriptGoogleAdsense style="display:block" data-ad-client="ca-pub-7284443005140816" data-ad-slot="8171782485"
          data-ad-format="auto" data-full-width-responsive />
      </v-col>
    </v-row>

    <h2 class="text-h6 text-sm-h5 mb-2 trending-title">Trending</h2>
    <media-trending :medias="trendingMedias"></media-trending>
  </v-container>
</template>

<script setup lang="ts">

import SearchComponent from "@/components/shows/SearchComponent.vue";
import logo from "@/components/icon/logo.vue";
import type { MediaDetailsDto, ShowDto } from "~/composables/api/data-contracts";
import { useMedia } from "~/composables/rest/api";
import { usePageLayout } from "~/composables/usePageLayout";
import { mdiSearchWeb } from "@mdi/js";

const TRENDING_LIMIT = 12;
const runtimeConfig = useRuntimeConfig();
const requestUrl = useRequestURL();
const siteOrigin = runtimeConfig.public.url || requestUrl.origin;

definePageMeta({
  name: "Home",
  order: 10,
  icon: mdiSearchWeb,
})
useSeoMeta({
  title: "Gestdown: Subtitle Aggregator",
  description: "Search and download subtitles from Addic7ed and SuperSubtitles in one place",
  ogDescription: "Search and download subtitles from Addic7ed and SuperSubtitles in one place",
  ogImage: new URL("/img/Gestdown-logos.jpeg", siteOrigin).href,
  ogLocale: "en-US",
  ogTitle: "Gestdown: Subtitle Aggregator",
  ogSiteName: "Gestdown: Subtitle Aggregator — Addic7ed & SuperSubtitles",
  keywords: "subtitles, Addic7ed, SuperSubtitles, subtitle aggregator, download subtitles",
})

const layout = usePageLayout();
const mediaApi = useMedia();

const { data, error } = await useAsyncData<MediaDetailsDto[]>(
  "home-trending",
  async () => (await mediaApi.trendingDetail(TRENDING_LIMIT)).data,
  {
    default: () => [],
  },
);

if (error.value != null) {
  console.error("can't get media trending", error.value);
  throw createError("Couldn't fetch trending medias");
}

const trendingMedias = data;

const goToPage = async (show: ShowDto) => {
  const router = useRouter();
  await router.push({ name: 'show-details', params: { showId: show.id, showName: show.slug } })
};

</script>

<style scoped>
.trending-title {
  text-shadow: 1px 1px 4px rgba(0, 0, 0, 0.8);
}
</style>

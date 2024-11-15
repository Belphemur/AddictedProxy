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
              <h2 class="text-h4">Search</h2>

              <SearchComponent
                  ref="searchBox"
                  v-on:selected="goToPage"
                  v-on:cleared="clear"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <h2 class="text-h4">Trending</h2>
            </v-col>
          </v-row>
          <v-row justify-lg="center">
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

import {ref} from "vue";
import SearchComponent from "@/components/shows/SearchComponent.vue";
import type {EpisodeWithSubtitlesDto, MediaDetailsDto, ShowDto} from "~/composables/api/data-contracts";
import {useMedia} from "~/composables/rest/api";
import type {SelectedShow} from "~/composables/dto/SelectedShow";
import {mdiSearchWeb} from "@mdi/js";

definePageMeta({
  name: "Home",
  order: 10,
  icon: mdiSearchWeb,
})
useSeoMeta({
  title: "Gestdown: Addic7ed Proxy",
  description: "Help you search for subtitle for different show available on Addic7ed",
  ogDescription: "Help you search for subtitle for different show available on Addic7ed",
  ogImage: new URL("/img/Gestdown-logos.jpeg", useRuntimeConfig().public.url).href,
  ogLocale: "en-US",
  ogTitle: "Gestdown: Addic7ed Proxy",
  ogSiteName: "Gestdown: Search and download subtitles from Addic7ed",
})

const mediaApi = useMedia();
const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto>>([]);
const searchBox = ref<InstanceType<typeof SearchComponent> | null>(null);
const currentShow = ref<SelectedShow | undefined>(undefined);


const {data, error} = await useAsyncData(async () => {
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
  await router.push({name: 'show-details', params: {showId: show.id, showName: show.slug}})
};

const clear = () => {
  episodesWithSubtitles.value = [];
  currentShow.value = undefined;
};

</script>

<style scoped>
</style>

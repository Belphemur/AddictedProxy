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
import {EpisodeWithSubtitlesDto, MediaDetailsDto, ShowDto} from "~/composables/api/data-contracts";
import {useMedia} from "~/composables/rest/api";
import {SelectedShow} from "~/composables/dto/SelectedShow";

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

const mediaApi = useMedia();
const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto>>([]);

const searchBox = ref<InstanceType<typeof SearchComponent> | null>(null);

const currentShow = ref<SelectedShow | undefined>(undefined);
const trendingMedias = ref<Array<MediaDetailsDto>>([]);



const mediaTrendingResponse = await mediaApi.trendingDetail(25)
trendingMedias.value = mediaTrendingResponse.data;



const goToPage = async (show: ShowDto) => {
  const router = useRouter();
  await router.push({name: 'show-details', params: {showId: show.id, showName: show.name}})
};

const clear = () => {
  episodesWithSubtitles.value = [];
  currentShow.value = undefined;
};

</script>

<style scoped>
</style>

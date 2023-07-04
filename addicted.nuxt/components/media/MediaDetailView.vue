<script setup lang="ts">
import {useApi} from "@/composables/rest/api";
import MediaDetails from "@/components/media/MediaDetails.vue";
import {SelectedShow} from "@/composables/dto/SelectedShow";
import {ref} from "vue";
import {EpisodeWithSubtitlesDto} from "@/api/api";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";


export interface Props {
  showId: string;
}

const props = defineProps<Props>();
const api = useApi();

const response = await api.media.mediaDetails(props.showId);
const mediaInfo = response.data;
const loadingEpisodes = ref(false);
const episodes = ref<EpisodeWithSubtitlesDto[]>([]);


useSeoMeta({
  title: `Gestdown: Subtitles of ${mediaInfo.media?.name}`,
  description: `Find all the subtitle you need for your favorite show ${mediaInfo.media?.name}`,
  ogDescription: `Find all the subtitle you need for your favorite show ${mediaInfo.media?.name}`,
  ogImage: mediaInfo.details?.posterPath
})

async function selected(selectedShow: SelectedShow) {
  loadingEpisodes.value = true;
  const response = await api.shows.showsDetail(selectedShow.show.id, selectedShow.season, selectedShow.language);
  episodes.value = response.data.episodes!;
  loadingEpisodes.value = false;
}
</script>

<template>
  <div>
    <v-row>
      <v-col cols="12">
        <media-details :details="mediaInfo" v-on:selected="selected"/>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-skeleton-loader type="card" :loading="loadingEpisodes">
          <subtitles-table :episodes="episodes"></subtitles-table>
        </v-skeleton-loader>
      </v-col>
    </v-row>
  </div>
</template>

<style scoped>

</style>
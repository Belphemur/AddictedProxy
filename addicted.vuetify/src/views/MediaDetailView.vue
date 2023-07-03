<script setup lang="ts">
import {useRouter} from "vue-router";
import {api} from "@/composables/rest/api";
import MediaDetails from "@/components/media/MediaDetails.vue";
import {SelectedShow} from "@/composables/dto/SelectedShow";
import {ref} from "vue";
import {EpisodeWithSubtitlesDto} from "@/api/api";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";

const router = useRouter();
const showId = router.currentRoute.value.params.showId;
const response = await api.media.mediaDetails(`${showId}`);
const mediaInfo = response.data;
const loadingEpisodes = ref(false);
const episodes = ref<EpisodeWithSubtitlesDto[]>([]);

async function selected(selectedShow: SelectedShow) {
  loadingEpisodes.value = true;
  const response = await api.shows.showsDetail(selectedShow.show.id, selectedShow.season, selectedShow.language);
  episodes.value = response.data.episodes!;
  loadingEpisodes.value = false;
}
</script>

<template>
  <v-container>
    <v-row>
      <media-details :details="mediaInfo" v-on:selected="selected"/>
    </v-row>
    <v-row>
      <v-skeleton-loader type="card" :loading="loadingEpisodes">
        <subtitles-table :episodes="episodes"></subtitles-table>
      </v-skeleton-loader>
    </v-row>
  </v-container>
</template>

<style scoped>

</style>

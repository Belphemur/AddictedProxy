<script setup lang="ts">

import {MediaDetailsDto} from "~/composables/api/data-contracts";

export interface Props {
  medias: Array<MediaDetailsDto>;
}

const props = defineProps<Props>();
</script>

<template>

  <v-row dense>
    <v-col cols="6" lg="3" v-for="media in useTake(props.medias, 8)"
           :key="media.media?.id">
      <v-card :to="{name: 'show-details', params: {showId: media.media!.id, showName: media.media!.name}}">
        <v-img
            gradient="to bottom, rgba(0,0,0,.1), rgba(0,0,0,1)"
            cover
            :src="media.details!.backdropPath!"
            :lazy-src="media.details!.backdropPath!"
            min-height="200px"
        >
          <v-row align="start" class="text">
            <v-col align-self="start">
              <v-card-title class="text-h6 text-white">
                <i>{{ media.details!.englishName }} ({{ media!.details?.releaseYear }})</i>
              </v-card-title>
            </v-col>
          </v-row>
          <v-row align="start" justify="start">
            <v-col align-self="start">
              <v-card-title class="text-white">
                <v-progress-circular color="white" bg-color="red" :size="70" :width="3"
                                     :model-value="media.details?.voteAverage *10">
                  {{ media.details?.voteAverage }}/10
                </v-progress-circular>
              </v-card-title>
            </v-col>
          </v-row>
        </v-img>

      </v-card>
    </v-col>
  </v-row>
</template>

<style>
.media-trending-backdrop img {
  max-width: 100%;
}
</style>
<style scoped>

.text {
  text-shadow: 2px 0 black, -2px 0 black, 0 2px black, 0 -2px black, 1px 1px black, -1px -1px black, -1px 1px black, 1px -1px black;
}
</style>
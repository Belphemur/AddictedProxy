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
      <v-card  :to="{name: 'show-details', params: {showId: media.media!.id, showName: media.media!.name}}">
        <span
            class="text-white text text-h6"
        >{{ media?.details.englishName }} ({{ media!.details?.releaseYear }})
        </span>
        <nuxt-img
            :src="media.details!.backdropPath!"
            fit="cover"
            style="max-width: 100%"
        >
        </nuxt-img>
        <v-progress-circular class="vote" color="white" bg-color="red" :size="60" :width="5"
                             :model-value="media.details?.voteAverage *10">
          {{ media.details?.voteAverage }}/10
        </v-progress-circular>
      </v-card>
    </v-col>
  </v-row>
</template>

<style scoped>

.text {
  position: absolute;
  left: 0.5em;
  top: 0.5em;
  text-shadow: 2px 0 black, -2px 0 black, 0 2px black, 0 -2px black, 1px 1px black, -1px -1px black, -1px 1px black, 1px -1px black;
}
.vote {
  position: absolute;
  left: 31em;
  top: 15em;
  text-shadow: 2px 0 black, -2px 0 black, 0 2px black, 0 -2px black, 1px 1px black, -1px -1px black, -1px 1px black, 1px -1px black;

}
</style>
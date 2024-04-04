<script setup lang="ts">


import type {MediaDetailsDto} from "~/composables/api/data-contracts";

export interface Props {
  medias: Array<MediaDetailsDto>;
}

const props = defineProps<Props>();
</script>

<template>

  <v-row dense>
    <v-col cols="12" sm="6" lg="3" v-for="media in useTake(props.medias, 8)"
           :key="media.media?.id">
      <v-card :to="{name: 'show-details', params: {showId: media.media!.id, showName: media.media!.slug}}">
        <span
            class="text-white text text-h6"
        >{{ media?.details.englishName }} ({{ media!.details?.releaseYear }})
        </span>
        <nuxt-picture
            :src="media.details!.backdropPath!"
            fit="cover"
            style="max-width: 100%"
            sizes="xs:100vw sm:35vw md:40vw"
            class="media-trending-backdrop"
        >
        </nuxt-picture>
        <v-progress-circular class="vote" color="white" bg-color="red" :size="60" :width="5"
                             :model-value="media.details?.voteAverage *10">
          {{ media.details?.voteAverage }}/10
        </v-progress-circular>
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
  position: absolute;
  left: 0.5em;
  top: 0.5em;
  text-shadow: 2px 0 black, -2px 0 black, 0 2px black, 0 -2px black, 1px 1px black, -1px -1px black, -1px 1px black, 1px -1px black;
}

.vote {
  position: absolute;
  left: 0.5em;
  top: 65%;
  text-shadow: 2px 0 black, -2px 0 black, 0 2px black, 0 -2px black, 1px 1px black, -1px -1px black, -1px 1px black, 1px -1px black;

}
</style>
<script setup lang="ts">


import type {MediaDetailsDto} from "~/composables/api/data-contracts";
import LazyOptimizedPicture from "~/components/image/LazyOptimizedPicture.vue";

export interface Props {
  medias: Array<MediaDetailsDto>;
}

const props = defineProps<Props>();
</script>

<template>

  <v-row justify="center">
    <v-col cols="12" sm="6" lg="3" v-for="media in props.medias"
           :key="media.media?.id"
           class="ma-0 pa-1"
           align-self="center">
      <NuxtLink :to="{name: 'show-details', params: {showId: media.media!.id, showName: media.media!.slug}}">
        <v-card>
        <span
            class="text-white text text-h6"
        >{{ media?.details.englishName }} ({{ media!.details?.releaseYear }})
        </span>
          <lazy-optimized-picture :src="media.details!.backdropPath!"
                                  :sources="[
                                { size: 'xl', width: 400, height: 225 },
                                { size: 'xxl', width: 500, height: 281 },
                           ]"
                                  :alt="`Backdrop poster for ${media.details!.englishName}`"
                                  :formats="[ 'webp', 'jpeg']"
                                  :placeholder-text="media.details!.englishName"
                                  class="media-trending-backdrop"
          />
          <v-progress-circular class="vote" color="white" bg-color="red" :size="60" :width="5"
                               :model-value="media.details?.voteAverage *10">
            {{ media.details?.voteAverage }}/10
          </v-progress-circular>
        </v-card>
      </NuxtLink>
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
  z-index: 10;
}

.vote {
  position: absolute;
  left: 0.5em;
  top: 65%;
  text-shadow: 2px 0 black, -2px 0 black, 0 2px black, 0 -2px black, 1px 1px black, -1px -1px black, -1px 1px black, 1px -1px black;
  z-index: 10;
}

.media-trending-backdrop img {
  max-width: 100%;
}
</style>
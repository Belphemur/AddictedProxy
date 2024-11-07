<script setup lang="ts">


import type {MediaDetailsDto} from "~/composables/api/data-contracts";
import OptimizedPicture from "~/components/image/OptimizedPicture.vue";

export interface Props {
  medias: Array<MediaDetailsDto>;
}

const props = defineProps<Props>();
</script>

<template>

  <v-row justify="center">
    <v-col cols="12" sm="6" lg="3" v-for="media in props.medias"
           :key="media.media?.id"
           class="ma-0 pa-0"
           align-self="center">
      <v-card :to="{name: 'show-details', params: {showId: media.media!.id, showName: media.media!.slug}}">
        <span
            class="text-white text text-h6"
        >{{ media?.details.englishName }} ({{ media!.details?.releaseYear }})
        </span>
        <optimized-picture :src="media.details!.backdropPath!"
                           :sources="[
                              { size: 'xs', width: 340,  height:191  },
                               { size: 'xl', width: 380, height:214 },
                           ]"
                           :alt="`Backdrop poster for ${media.details!.englishName}`"
                           :formats="[ 'webp', 'jpeg']"
                           preload
                           class="media-trending-backdrop"
        />
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

.media-trending-backdrop img {
  max-width: 100%;
}
</style>
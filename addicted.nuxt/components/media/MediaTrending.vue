<script setup lang="ts">

import {MediaDetailsDto} from "~/composables/api/data-contracts";

export interface Props {
  medias: Array<MediaDetailsDto>;
}

const props = defineProps<Props>();
</script>

<template>
  <v-carousel cycle height="400" hide-delimiters show-arrows="hover">

    <v-carousel-item v-for="media in props.medias"
                     :key="media.media?.id">
      <v-card :to="{name: 'show-details', params: {showId: media.media!.id, showName: media.media!.name}}">
        <v-img
            :src="media.details!.backdropPath!"
            gradient="to bottom, rgba(0,0,0,.1), rgba(0,0,0,1)"
            height="100%"
            cover
        >
          <v-card-title
              class="text-white font-weight-black"
          >{{ media?.details.englishName }} ({{ media!.details?.releaseYear }})
          </v-card-title>
        </v-img>
      </v-card>
    </v-carousel-item>
  </v-carousel>
</template>

<style scoped>

</style>
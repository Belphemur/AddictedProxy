<script setup lang="ts">

import type { MediaDetailsDto } from "~/composables/api/data-contracts";
import { langs } from "~/composables/language/lang";
import LazyOptimizedPicture from "~/components/image/LazyOptimizedPicture.vue";
import { mdiMovie, mdiTelevision } from "@mdi/js";

export interface Props {
  details: MediaDetailsDto;
}

const language = useLanguage();
const props = defineProps<Props>();
const seasons = computed<Number[]>(() => props.details.media!.seasons!)

const season = defineModel<number>();

const setLanguage = (lang: string) => {
  if (lang == language.lang) {
    return;
  }
  if (lang == null || lang == "" || lang == undefined) {
    return;
  }
  language.lang = lang;
}

</script>

<template>
  <v-sheet rounded="lg" color="rgba(0,0,0,0.75)" class="pa-4 pa-sm-5">
    <div v-once class="mb-1">
      <h1 class="text-h5">
        <v-icon>{{ props.details.details?.mediaType === 'Movie' ? mdiMovie : mdiTelevision }}</v-icon>
        {{ props.details.details!.englishName }} <span class="text-light-blue-accent-1 font-bold"
          v-if="props.details.details?.releaseYear != null">({{ props.details.details.releaseYear }})</span>
        <span v-if="props.details.details?.englishName != props.details.details?.originalName">
          [<i>{{ props.details.details.originalName }}</i>]
        </span>
      </h1>
      <p class="text-medium-emphasis mt-1"><i>{{ props.details.details.tagLine }}</i></p>
    </div>
    <div v-once>
      <v-row dense align="center">
        <v-col cols="12" sm="2" class="text-left" align-self="start">
          <lazy-optimized-picture :src="props.details.details!.posterPath!" preload class="backdrop-image"
            :placeholder-text="props.details.details!.englishName" :sources="[
              {
                size: 'xs',
                height: 180,
                width: 320,
                media: '(orientation: portrait)',
                src: props.details.details!.backdropPath!
              }, {
                size: 'sm',
                width: 200,
                height: 300,
                media: '(orientation: portrait)',
                src: props.details.details!.posterPath!
              },
              {
                size: 'xs',
                width: 350,
                height: 525,
                media: '(orientation: landscape)',
              }, {
                size: 'sm',
                width: 250,
                height: 375,
                media: '(orientation: landscape)',
              }, {
                size: 'xl',
                width: 260,
                height: 390
              },
              {
                size: 'xxl',
                width: 300,
                height: 450
              }]" :alt="`Poster for ${props.details.details?.englishName}`" :formats="['webp', 'jpeg']" />
        </v-col>

        <v-col sm="10" cols="12" class="text-left">
          <v-row dense align-content="center">
            <v-col>
              <h6 class="text-subtitle-1 font-weight-bold">User Score</h6>
              <v-progress-circular color="yellow" :size="64" :width="12"
                :model-value="props.details.details!.voteAverage * 10">
                {{ props.details.details!.voteAverage }}
              </v-progress-circular>
            </v-col>
            <v-col>
              <h6 class="text-subtitle-1 font-weight-bold">Genres</h6>
              {{ props.details.details!.genre.join(", ") }}
            </v-col>
          </v-row>
          <v-row dense>
            <v-col align-self="end">
              <h6 class="text-subtitle-1 font-weight-bold">Overview</h6>
              <p class="overview-clamp" :title="props.details.details!.overview">
                {{ props.details.details!.overview }}
              </p>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </div>
    <v-row dense class="mt-1">
      <v-col xl="4" cols="6">
        <v-autocomplete :model-value="language.lang" @update:model-value="setLanguage" :items="langs" label="Language"
          item-title="name" item-value="code" clearable="" hide-details></v-autocomplete>
      </v-col>
      <v-col xl="4" cols="6" offset-xl="4">
        <v-select v-model="season" :items="seasons" label="Season selection" hide-details></v-select>
      </v-col>
    </v-row>
  </v-sheet>
</template>

<style>
.backdrop-image img {
  margin: auto;
  display: block;
}
</style>

<style scoped>
.overview-clamp {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

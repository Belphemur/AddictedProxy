<script setup lang="ts">

import type {MediaDetailsDto} from "~/composables/api/data-contracts";
import {langs} from "~/composables/language/lang";
import OptimizedPicture from "~/components/image/OptimizedPicture.vue";

export interface Props {
  details: MediaDetailsDto;
}

const language = useLanguage();
const props = defineProps<Props>();
const seasons = props.details.media!.seasons!;

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
  <v-card>
    <v-card-title v-once>
      <v-col>
        <h1 class="text-h5">
          <v-icon :icon=" props.details.details?.mediaType === 'Movie' ? 'mdi-movie' : 'mdi-television'"></v-icon>
          {{ props.details.details!.englishName }} <span
            class="text-light-blue-accent-1 font-bold"
            v-if="props.details.details?.releaseYear != null">({{ props.details.details.releaseYear }})</span>
          <span v-if="props.details.details?.englishName != props.details.details?.originalName">
          [<i>{{ props.details.details.originalName }}</i>]
      </span>
        </h1>
      </v-col>
    </v-card-title>
    <v-card-subtitle v-once>
      <i>{{ props.details.details.tagLine }}</i>
    </v-card-subtitle>
    <v-card-text class="py-0" v-once>
      <v-row align="center">
        <v-col
            cols="12"
            sm="4"
            class="text-left"
            align-self="start"
        >
          <optimized-picture :src="props.details.details!.posterPath!"
                             preload
                             class="backdrop-image"
                             :sources="[
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
                               height: 525
                             }, {
                               size: 'sm',
                               width: 250,
                               height: 375
                             }, {
                               size: 'lg',
                               width: 230,
                               height: 345
                             },
                              {
                               size: 'xl',
                               width: 300,
                               height:450
                             }]"
                             :alt="`Poster for ${props.details.details?.englishName}`"
                             :formats="['webp', 'jpeg']"/>

        </v-col>

        <v-col sm="8" cols="12" class="text-left">
          <v-row align-content="center">
            <v-col>
              <h6 class="text-h6">User Score</h6>
              <v-progress-circular color="yellow" :size="80" :width="15"
                                   :model-value="props.details.details?.voteAverage *10">
                {{ props.details.details?.voteAverage }}
              </v-progress-circular>
            </v-col>
            <v-col>
              <h6 class="text-h6">Genres</h6>
              {{ props.details.details?.genre.join(", ") }}
            </v-col>
          </v-row>
          <v-row>
            <v-col align-self="end">
              <h5 class="text-h5">Overview</h5>
              <p>
                {{ props.details.details!.overview }}
              </p>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-col xl="4" cols="6">
        <v-autocomplete :model-value="language.lang"
                        @update:model-value="setLanguage"
                        :items="langs"
                        label="Language"
                        item-title="name"
                        item-value="code"
                        clearable=""
        ></v-autocomplete>
      </v-col>
      <v-col xl="4" cols="6" offset-xl="4">
        <v-select v-model="season"
                  :items="seasons"
                  label="Season selection"
        ></v-select>
      </v-col>
    </v-card-actions>
  </v-card>
</template>

<style>
.backdrop-image img {
  margin: auto;
  display: block;
  max-width: 100%;
}
</style>

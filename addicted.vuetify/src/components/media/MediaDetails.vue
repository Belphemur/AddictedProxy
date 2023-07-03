<script setup lang="ts">

import {MediaDetailsDto} from "@/api/api";

export interface Props {
  details: MediaDetailsDto;
}

const props = defineProps<Props>();
</script>

<template>
  <v-card>
    <v-card-title>
      <v-icon :icon=" props.details.details?.mediaType === 'Movie' ? 'mdi-movie' : 'mdi-television'"></v-icon>
      {{ props.details.media!.name }}
    </v-card-title>
    <v-card-subtitle v-if="props.details.media?.name != props.details.details?.originalName">
      <i>{{ props.details.details.originalName }}</i>
    </v-card-subtitle>
    <v-card-text class="py-0">
      <v-row align="center" no-gutters>
        <v-col
          class="text-h2"
          cols="2"
        >
          <v-img class="align-end ml-auto" :src="props.details.details!.posterPath!" height="320"></v-img>
        </v-col>

        <v-col cols="10" class="text-right">
          {{ props.details.details!.overview }}
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<style scoped>

</style>

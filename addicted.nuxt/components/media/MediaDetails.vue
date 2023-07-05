<script setup lang="ts">

import {MediaDetailsDto} from "~/composables/api/api";
import {ref, watch} from "vue";
import {getAll639_1, getName} from "all-iso-language-codes";
import {SelectedShow} from "@/composables/dto/SelectedShow";

export interface Props {
  details: MediaDetailsDto;
}

const language = useLanguage();
const props = defineProps<Props>();
const seasons = props.details.media!.seasons!;
const selectedSeason = ref<number>(seasons[seasons.length - 1]);

const languageSelect = ref<string>(language.lang);

const langs = getAll639_1().map((value) => {
  return {value: value, label: getName(value)};
});


async function emitSelected() {
  emit("selected", {
    show: props.details.media!,
    season: selectedSeason.value,
    language: languageSelect.value,
  })
}

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", show: SelectedShow): void;
}>();

watch(selectedSeason, async (value) => {
  if (value == null) {
    return;
  }
  await emitSelected();

});

watch(languageSelect, async (value) => {
  if (value == null) {
    return;
  }
  language.lang = value;
  await emitSelected();
});
</script>

<template>
  <v-card>
    <v-card-title>
      <v-icon :icon=" props.details.details?.mediaType === 'Movie' ? 'mdi-movie' : 'mdi-television'"></v-icon>
      {{ props.details.media!.name }} <span class="text-light-blue-accent-1 font-bold">({{props.details.details.releaseYear}})</span>
      <span v-if="props.details.media?.name != props.details.details?.originalName">
          [<i>{{ props.details.details.originalName }}</i>]
      </span>
    </v-card-title>
    <v-card-subtitle>
      <i>{{ props.details.details.tagLine }}</i>
    </v-card-subtitle>
    <v-card-text class="py-0">
      <v-row align="center">
        <v-col
            cols="4"
            class="text-left"
            align-self="start"
        >
          <v-img min-height="320" max-height="450" :src="props.details.details!.posterPath!" ></v-img>
        </v-col>

        <v-col cols="8" class="text-left">
          <v-row align-content="center">
            <v-col>
              <h6 class="text-h6">User Score</h6>
              <v-progress-circular color="yellow" :size="80" :width="15"
                                   :model-value="props.details.details?.voteAverage *10">
                {{ props.details.details?.voteAverage }}
              </v-progress-circular>
            </v-col>
          </v-row>
          <v-row>
            <v-col align-self="end">
              <h4 class="text-h4">Overview</h4>
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
        <v-autocomplete v-model="languageSelect"
                        :items="langs"
                        label="Language"
                        item-title="label"
                        item-value="value"
        ></v-autocomplete>
      </v-col>
      <v-col xl="4" cols="6" offset-xl="4">
        <v-select v-model="selectedSeason"
                  :items="seasons"
                  label="Season selection"
        ></v-select>
      </v-col>
    </v-card-actions>
  </v-card>
</template>

<style scoped>

</style>

<template>
  <v-container>
    <v-row>
      <v-col>
        <v-autocomplete label="Name of the show"
                        clearable
                        :error-messages="error"
                        :items="results"
                        :loading="isLoading"
                        item-title="name"
                        hide-no-data
                        :item-value="item => item"
                        @update:search="onSearch"
                        @update:modelValue="updateSelectedShow"
                        :prepend-inner-icon="mdiTelevision"
                        @click:clear="clearSearch"
        ></v-autocomplete>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import {ref,} from "vue";
import {mevent} from "~/composables/data/event";
import {useShows} from "~/composables/rest/api";
import type {ShowDto} from "~/composables/api/data-contracts";
import {mdiTelevision} from "@mdi/js";

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", show: ShowDto): void,
  (e: "cleared"): void;
}>();

const showsApi = useShows();

const selectedSeason = ref<number | null>(null);

const selectedShow = ref<ShowDto | null>(null);

const selectedShowSeason = ref<Array<number>>([]);

const results = ref<ShowDto[]>([]);
const isLoading = ref(false);

let timerId: number = 0;

const error = ref<string | undefined>(undefined);

const clearSearch = () => {
  selectedShow.value = null;
  selectedSeason.value = null;
  selectedShowSeason.value = [];
  emit("cleared");
};
const onSearch = async (val: string) => {
  if (process.server) {
    return;
  }
  clearTimeout(timerId)
  timerId = setTimeout(async () => {
    isLoading.value = true;
    results.value = await querySearch(val);
    isLoading.value = false;
  }, 350);

};


const querySearch = async (query: string) => {
  error.value = undefined;
  if (query.length < 3) {
    return [];
  }
  mevent("show-search", {query: query});
  try {
    const searchResponse = await showsApi.searchDetail(query);
    const shows = searchResponse.data.shows;
    if (shows == null)
      return [];
    return shows;
  } catch (e) {
    error.value = `Couldn't find show: ${query}`;
    return [];
  }
};

const updateSelectedShow = async (event: ShowDto) => {
  if (event == null) {
    return;
  }
  if (event.seasons != undefined && event.seasons.length > 1) {
    event.seasons = event.seasons.sort();
  }

  mevent("show-selected", {show: event});
  emit("selected", event);
};

</script>


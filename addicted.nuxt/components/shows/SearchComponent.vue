<template>
  <v-container>
    <v-row>
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
                      prepend-inner-icon="mdi-television"
                      @click:clear="clearSearch"
      ></v-autocomplete>
    </v-row>
    <v-slide-y-transition>
      <v-row v-show="selectedShow != null">
        <v-col cols="12" md="6">
          <v-autocomplete v-model="languageSelect"
                          :items="langs"
                          label="Language"
                          item-title="label"
                          item-value="value"
          ></v-autocomplete>
        </v-col>
        <v-col cols="12" md="6">
          <v-select v-model="selectedSeason"
                    :items="selectedShowSeason"
                    label="Season selection"
          ></v-select>
        </v-col>
      </v-row>
    </v-slide-y-transition>
  </v-container>
</template>

<script setup lang="ts">
import {ref, watch, defineExpose} from "vue";
import {ShowDto} from "~/composables/api/api";
import {getName, getAll639_1} from "all-iso-language-codes";
import {SelectedShow} from "@/composables/dto/SelectedShow";
import {mevent} from "~/composables/data/event";
import {useApi} from "~/composables/rest/api";

const langs = getAll639_1().map((value) => {
  return {value: value, label: getName(value)};
});

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", show: SelectedShow): void;
  (e: "cleared"): void;
  (e: "needRefresh", show: ShowDto): void;
}>();

const api = useApi();

const language = useLanguage();

const selectedSeason = ref<number | null>(null);
const languageSelect = ref<string>(language.lang);

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
    const searchResponse = await api.shows.searchDetail(query);
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

  setSelectedShow(event);
  mevent("show-selected", {show: event});
  //Force refreshing the show
  if (selectedShow.value!.nbSeasons == 0) {
    emit("needRefresh", selectedShow.value!);
    selectedShow.value = null;
    selectedShowSeason.value = [];
    return;
  }
};

const setSelectedShow = (show: ShowDto) => {
  if (show == null) {
    return;
  }
  selectedShow.value = show;
  selectedSeason.value = null;
  selectedShowSeason.value = show.seasons;
  if (show.nbSeasons > 0) {
    const lastSeason = selectedShowSeason.value.length - 1;
    selectedSeason.value = selectedShowSeason.value[lastSeason];
  }
};

defineExpose({setSelectedShow});

watch(selectedSeason, (value) => {
  if (value == null || selectedShow.value == null) {
    emit("cleared");
    return;
  }
  emit("selected", {
    language: languageSelect.value,
    season: value,
    show: selectedShow.value,
  });
});

watch(languageSelect, (value) => {
  language.lang = value;
  if (selectedSeason.value == null || selectedShow.value == null) {
    return;
  }
  emit("selected", {
    language: value,
    season: selectedSeason.value,
    show: selectedShow.value,
  });
});
</script>


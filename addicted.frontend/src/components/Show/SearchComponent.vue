<template>
  <el-autocomplete
    v-model.lazy="searchInput"
    :fetch-suggestions="querySearch"
    clearable
    placeholder="Name of the show"
    size="large"
    :debounce="500"
    :trigger-on-focus="false"
    v-on:select="updateSelectedShow"
    :minlength="5"
  >
    <template #prepend>
      <el-select
        v-model="languageSelect"
        placeholder="Select"
        style="width: 115px"
        filterable
      >
        <el-option
          v-for="item in langs"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        />
      </el-select>
    </template>
    <template #append>
      <el-select
        v-if="selectedShowSeason.length > 0"
        v-model="selectedSeason"
        placeholder="Season"
      >
        <el-option
          v-for="item in selectedShowSeason"
          :key="item"
          :label="`Season ${item}`"
          :value="item"
        />
      </el-select>
    </template>
    <template #default="{ item }">
      <span class="name">{{ item.name }}</span>
      <span v-if="item.seasons.length > 0"> ({{ item.seasons.length }})</span>
    </template>
  </el-autocomplete>
</template>

<script setup lang="ts">
import { ref, watch, defineExpose } from "vue";
import { TvShowsApi, Configuration, ShowDto } from "@/api";
import { getName, getAll639_1 } from "all-iso-language-codes";
import { SelectedShow } from "@/Dto/SelectedShow";

const langs = getAll639_1().map((value) => {
  return { value: value, label: getName(value) };
});
const userLang = (navigator.language || navigator.userLanguage).split("-")[0];

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", show: SelectedShow): void;
  (e: "cleared"): void;
  (e: "needRefresh", show: ShowDto): void;
}>();

const selectedSeason = ref<number | null>(null);
const searchInput = ref<string>("");
const languageSelect = ref<string>(userLang);
const api = new TvShowsApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);

const selectedShow = ref<ShowDto | null>(null);

const selectedShowSeason = ref<Array<number>>([]);

const querySearch = async (query: string, cb: (param: unknown) => void) => {
  const searchResponse = await api.showsSearchPost({ query: query });
  cb(searchResponse.shows);
};

const updateSelectedShow = async (event: ShowDto) => {
  setSelectedShow(event);

  //Force refreshing the show
  if (selectedShow.value!.nbSeasons == 0) {
    emit("needRefresh", selectedShow.value!);
    selectedShow.value = null;
    selectedShowSeason.value = [];
    return;
  }
};

const setSelectedShow = (show: ShowDto) => {
  selectedShow.value = show;
  searchInput.value = show.name;
  selectedSeason.value = null;
  selectedShowSeason.value = show.seasons;
};

defineExpose({ setSelectedShow });

watch(searchInput, (value) => {
  if (value == "") {
    selectedSeason.value = null;
    selectedShowSeason.value = [];
  }
});

watch(selectedSeason, (value) => {
  if (value == null || selectedShow.value == null) {
    emit("cleared");
    return;
  }
  emit("selected", {
    language: languageSelect.value,
    season: value,
    showId: selectedShow.value.id,
    name: selectedShow.value.name,
  });
});

watch(languageSelect, (value) => {
  if (selectedSeason.value == null || selectedShow.value == null) {
    return;
  }
  emit("selected", {
    language: value,
    season: selectedSeason.value,
    showId: selectedShow.value.id,
    name: selectedShow.value.name,
  });
});
</script>

<style>
.el-autocomplete {
  align-self: center;
  flex-grow: 1;
}
</style>

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
      <el-tooltip
        effect="dark"
        content="Search a show first"
        placement="right"
        :disabled="selectedShowSeason.length > 0"
      >
        <el-select
          v-model="selectedSeason"
          placeholder="Select a season"
          :disabled="selectedShowSeason.length === 0"
        >
          <el-option
            v-for="item in selectedShowSeason"
            :key="item"
            :label="`Season ${item}`"
            :value="item"
          />
        </el-select>
      </el-tooltip>
    </template>
    <template #default="{ item }">
      <span class="name">{{ item.name }}</span>
      <span v-if="item.seasons.length > 0"> ({{ item.seasons.length }})</span>
    </template>
  </el-autocomplete>
</template>

<script setup lang="ts">
import { ref, watch, defineExpose } from "vue";
import { ShowDto } from "~/api/api";
import { getName, getAll639_1 } from "all-iso-language-codes";
import { SelectedShow } from "~/Dto/SelectedShow";
import { api } from "~/composables/rest/api";
import { mevent } from "~/composables/matomo/event";

const langs = getAll639_1().map((value) => {
  return { value: value, label: getName(value) };
});

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", show: SelectedShow): void;
  (e: "cleared"): void;
  (e: "needRefresh", show: ShowDto): void;
}>();

const selectedSeason = ref<number | null>(null);
const searchInput = ref<string>("");
const languageSelect = ref<string>(localStorage.getItem("lang") || "en");

const selectedShow = ref<ShowDto | null>(null);

const selectedShowSeason = ref<Array<number>>([]);

const querySearch = async (query: string, cb: (param: unknown) => void) => {
  if (query.length < 3) {
    cb([]);
    return;
  }
  mevent("show-search", { query: query });
  const searchResponse = await api.shows.searchCreate({ query: query });
  cb(searchResponse.data.shows);
};

const updateSelectedShow = async (event: ShowDto) => {
  setSelectedShow(event);
  mevent("show-selected", { show: event });
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
  if (show.nbSeasons > 0) {
    const lastSeason = selectedShowSeason.value.length - 1;
    selectedSeason.value = selectedShowSeason.value[lastSeason];
  }
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
    show: selectedShow.value,
  });
});

watch(languageSelect, (value) => {
  localStorage.setItem("lang", value);
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

<style>
.el-autocomplete {
  align-self: center;
  flex-grow: 1;
}
</style>

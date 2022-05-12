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
        v-show="selectedShowSeason.length > 0"
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
      <span class="name">{{ item.title }}</span>
      <span v-if="item.seasons > 0"> ({{ item.seasons }})</span>
    </template>
  </el-autocomplete>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { TvShowsApi, Configuration } from "@/api";
import { ShowDto } from "@/Dto/ShowDto";
import { getName, getAll639_1 } from "all-iso-language-codes";
import { SelectedShow } from "@/Dto/SelectedShow";
import { ElMessage, ElNotification } from "element-plus";

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
  cb(
    searchResponse.shows?.map((show) => {
      return {
        title: show.name,
        id: show.id,
        seasons: show.nbSeasons,
      } as ShowDto;
    })
  );
};

const updateSelectedShow = async (event: ShowDto) => {
  selectedShow.value = event;
  selectedSeason.value = null;
  //Force refreshing the show
  if (selectedShow.value.seasons == 0) {
    emit("needRefresh", {
      ...selectedShow.value,
    });
    selectedShow.value = null;
    selectedShowSeason.value = [];
    return;
  }
  selectedShowSeason.value = Array.from(
    { length: event.seasons },
    (_, i) => i + 1
  );
};

watch(searchInput, (value) => {
  if (value == "") {
    selectedSeason.value = null;
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
  });
});
</script>

<style>
.el-autocomplete {
  align-self: center;
  flex-grow: 1;
}
</style>

<template>
  <div class="mt-4">
    <el-autocomplete
      v-model="searchInput"
      :fetch-suggestions="querySearch"
      clearable
      placeholder="Search for your episode: Wellington S01E01"
      class="inline-input"
      size="large"
      :debounce="500"
      :trigger-on-focus="false"
      v-on:select="updateSelectedShow"
    >
      <template #prepend>
        <el-select
          v-model="languageSelect"
          placeholder="Select"
          style="width: 115px"
        >
          <el-option label="English" value="en" />
        </el-select>
      </template>
      <template #default="{ item }">
        <span class="name">{{ item.value }}</span>
        <span v-if="item.seasons > 0"> ({{ item.seasons }})</span>
      </template>
    </el-autocomplete>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { TvShowsApi, Configuration } from "@/api";
import { ShowDto } from "@/Dto/ShowDto";

const searchInput = ref("");
const userLang = (navigator.language || navigator.userLanguage).split("-")[0];
const languageSelect = ref(userLang);
const api = new TvShowsApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);

const selectedShow = ref<ShowDto | null>(null);

const querySearch = async (query: string, cb: (param: unknown) => void) => {
  const searchResponse = await api.showsSearchPost({ query: query });
  cb(
    searchResponse.shows?.map((show) => {
      return {
        value: show.name,
        id: show.id,
        seasons: show.nbSeasons,
      } as unknown as ShowDto;
    })
  );
};

const updateSelectedShow = async (event: ShowDto) => {
  selectedShow.value = event;
  //Force refreshing the show
  if (selectedShow.value.seasons == 0) {
    await api.showsRefreshShowIdPost(selectedShow.value.id);
  }
};
</script>

<style scoped></style>

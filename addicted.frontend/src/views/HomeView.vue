<template>
  <el-row>
    <el-col :offset="5" :span="14">
      <SearchComponent
        v-on:selected="getSubtitles"
        v-on:cleared="clear"
        style="display: flex; flex-grow: 1"
      />
    </el-col>
  </el-row>
  <el-row>
    <el-col :offset="5" :span="14">
      <subtitles-table
        :episodes="episodesWithSubtitles"
        v-show="
          episodesWithSubtitles != null && episodesWithSubtitles.length > 0
        "
        style="display: flex; flex-grow: 1"
      ></subtitles-table>
    </el-col>
  </el-row>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { SelectedShow } from "@/Dto/SelectedShow";
import SubtitlesTable from "@/components/Show/SubtitlesTable.vue";
import { Configuration, EpisodeWithSubtitlesDto, TvShowsApi } from "@/api";

const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto> | null>(null);
const api = new TvShowsApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);

const getSubtitles = async (show: SelectedShow) => {
  const response = await api.showsShowIdSeasonNumberLanguageGet(
    show.showId,
    show.season,
    show.language
  );
  episodesWithSubtitles.value = response.episodes || null;
};

const clear = () => {
  episodesWithSubtitles.value = null;
};
</script>

<style scoped></style>

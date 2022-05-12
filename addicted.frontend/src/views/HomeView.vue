<template>
  <el-row>
    <el-col :offset="5" :span="14">
      <p>Welcome to Gestdown.</p>
      <p>
        It acts as a proxy for the Addic7ed subtitle website. You can easily
        search here for subtitle available on the platform and download them.
      </p>
      <el-divider>
        <el-icon>
          <search />
        </el-icon>
      </el-divider>
      <SearchComponent
        v-on:selected="getSubtitles"
        v-on:cleared="clear"
        v-on:need-refresh="needRefresh"
        style="display: flex; flex-grow: 1"
      />
    </el-col>
  </el-row>
  <el-row>
    <el-col :offset="5" :span="14">
      <el-progress
        v-show="refreshShowProgress > 0"
        :percentage="refreshShowProgress"
        :format="formatPercentage"
      />
      <el-divider>
        <el-icon>
          <arrow-down-bold />
        </el-icon>
      </el-divider>
      <el-skeleton :rows="5" animated :loading="loadingSubtitles">
        <template #default>
          <subtitles-table
            v-show="episodesWithSubtitles.length > 0"
            :episodes="episodesWithSubtitles"
            style="display: flex; flex-grow: 1"
          ></subtitles-table>
          <el-empty
            description="No Result"
            v-show="episodesWithSubtitles.length == 0"
          />
        </template>
      </el-skeleton>
    </el-col>
  </el-row>
</template>

<script setup lang="ts">
import { onUnmounted, ref } from "vue";
import { SelectedShow } from "@/Dto/SelectedShow";
import SubtitlesTable from "@/components/Show/SubtitlesTable.vue";
import { Configuration, EpisodeWithSubtitlesDto, TvShowsApi } from "@/api";
import { Search, ArrowDownBold } from "@element-plus/icons-vue";
import { ElMessage } from "element-plus";
import {
  offProgress,
  onProgress,
  ProgressHandler,
  sendRefreshAsync,
} from "@/composables/hub/RefreshHub";
import { ShowDto } from "@/Dto/ShowDto";

const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto>>([]);
const api = new TvShowsApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);

const loadingSubtitles = ref(false);
const refreshShowProgress = ref(0);
const refreshShowName = ref("");

const getSubtitles = async (show: SelectedShow) => {
  loadingSubtitles.value = true;
  const response = await api.showsShowIdSeasonNumberLanguageGet(
    show.showId,
    show.season,
    show.language
  );
  episodesWithSubtitles.value = response.episodes || [];
  loadingSubtitles.value = false;
};

const clear = () => {
  episodesWithSubtitles.value = [];
};

const formatPercentage = (progress: number) =>
  `Fetching ${refreshShowName.value}: (${progress}%)`;

const needRefresh = async (show: ShowDto) => {
  refreshShowName.value = show.title;
  ElMessage({
    message:
      "We don't have any subtitle for that show. We're fetching them from Addic7ed.\nPlease Try later.",
    type: "warning",
    duration: 5000,
  });
  await sendRefreshAsync(show.id);
};
const progressHandler: ProgressHandler = (progress) => {
  refreshShowProgress.value = progress.progress;
};
onProgress(progressHandler);
onUnmounted(() => {
  offProgress(progressHandler);
});
</script>

<style scoped></style>

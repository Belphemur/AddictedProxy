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
      <el-divider v-show="refreshingShows.size > 0">
        <el-icon>
          <refresh />
        </el-icon>
      </el-divider>
      <el-progress
        v-for="[key, value] in refreshingShows"
        v-bind:key="key"
        :percentage="value.progress"
        :format="formatPercentage(key)"
        :text-inside="true"
        :stroke-width="24"
        class="progress-bar"
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
import { Search, ArrowDownBold, Refresh } from "@element-plus/icons-vue";
import { ElMessage } from "element-plus";
import {
  offProgress,
  onProgress,
  ProgressHandler,
  sendRefreshAsync,
} from "@/composables/hub/RefreshHub";
import { ShowDto } from "@/Dto/ShowDto";

interface ProgressShow {
  name: string;
  progress: number;
}

const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto>>([]);
const api = new TvShowsApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);

const loadingSubtitles = ref(false);
const refreshingShows = ref(new Map<string, ProgressShow>());

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
const formatPercentage = (showId: string) => {
  const showProgress = refreshingShows.value.get(showId)!;

  return (progress: number) => {
    return `Fetching ${showProgress.name}: (${progress}%)`;
  };
};

const needRefresh = async (show: ShowDto) => {
  ElMessage({
    message:
      "We don't have any subtitle for that show. We're fetching them from Addic7ed.\nPlease Try later.",
    type: "warning",
    duration: 5000,
  });
  refreshingShows.value.set(show.id, { name: show.title, progress: 0 });
  await sendRefreshAsync(show.id);
};
const progressHandler: ProgressHandler = (progress) => {
  const progressShow = refreshingShows.value.get(progress.showId)!;
  refreshingShows.value.set(progress.showId, {
    ...progressShow,
    progress: progress.progress,
  });
};
onProgress(progressHandler);
onUnmounted(() => {
  offProgress(progressHandler);
});
</script>

<style scoped>
.progress-bar {
  padding-bottom: 0.5rem;
}
</style>

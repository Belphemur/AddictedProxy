<template>
  <el-row>
    <el-col :offset="5" :span="14">
      <el-card header="Welcome to Gestdown">
        <p>
          It acts as a proxy for the Addic7ed subtitle website. You can easily
          search here for subtitle available on the platform and download them.
        </p>

        <SearchComponent
          ref="searchBox"
          v-on:selected="getSubtitles"
          v-on:cleared="clear"
          v-on:need-refresh="needRefresh"
          style="display: flex; flex-grow: 1"
        />
      </el-card>
    </el-col>
  </el-row>
  <el-row>
    <el-col :offset="5" :span="14">
      <el-divider v-if="refreshingShows.size > 0">
        <el-icon>
          <refresh />
        </el-icon>
      </el-divider>
      <div
        v-for="[key, value] in refreshingShows"
        v-bind:key="key"
        class="progress-container"
      >
        <el-progress
          :percentage="value.progress"
          :format="formatPercentage(key)"
          :text-inside="true"
          :stroke-width="32"
          :class="`progress-bar${value.show != null ? '-with-button' : ''}`"
        />
        <el-tooltip
          v-if="value.show != null"
          class="box-item"
          effect="dark"
          content="Redo the search"
          placement="right"
        >
          <el-button
            class="search-button"
            :icon="Search"
            type="primary"
            circle
            @click="selectShow(key)"
          />
        </el-tooltip>
      </div>
      <el-divider>
        <el-icon>
          <search />
        </el-icon>
      </el-divider>
    </el-col>
  </el-row>
  <el-row>
    <el-col :offset="5" :span="14">
      <el-card>
        <template #header v-if="currentShow != null">
          <div class="card-header">
            <span>
              {{ currentShow.show.name }}: Season
              {{ currentShow.season }}
            </span>
            <el-tooltip
              effect="dark"
              content="Fetch from Addic7ed"
              placement="right"
            >
              <el-button
                class="button"
                type="primary"
                :icon="Refresh"
                @click="refreshShow(currentShow.show)"
              />
            </el-tooltip>
          </div>
        </template>
        <el-skeleton :rows="5" animated :loading="loadingSubtitles">
          <template #default>
            <subtitles-table
              v-if="episodesWithSubtitles.length > 0"
              :episodes="episodesWithSubtitles"
              style="display: flex; flex-grow: 1"
            ></subtitles-table>
            <el-empty
              description="Subtitles will be shown here"
              v-if="episodesWithSubtitles.length == 0"
            />
          </template>
        </el-skeleton>
      </el-card>
    </el-col>
  </el-row>
</template>

<script setup lang="ts">
import SearchComponent from "@/components/Show/SearchComponent.vue";
import { onUnmounted, ref } from "vue";
import { SelectedShow } from "@/Dto/SelectedShow";
import SubtitlesTable from "@/components/Show/SubtitlesTable.vue";
import {
  Configuration,
  EpisodeWithSubtitlesDto,
  ShowDto,
  TvShowsApi,
} from "@/api";
import { Search, ArrowDownBold, Refresh } from "@element-plus/icons-vue";
import { ElMessage } from "element-plus";
import {
  DoneHandler,
  offDone,
  offProgress,
  onDone,
  onProgress,
  ProgressHandler,
  sendRefreshAsync,
  unsubscribeShowAsync,
} from "@/composables/hub/RefreshHub";

interface ProgressShow {
  name: string;
  progress: number;
  show?: ShowDto;
}

const episodesWithSubtitles = ref<Array<EpisodeWithSubtitlesDto>>([]);
const api = new TvShowsApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);

const searchBox = ref<InstanceType<typeof SearchComponent> | null>(null);

const loadingSubtitles = ref(false);
const refreshingShows = ref(new Map<string, ProgressShow>());
const currentShow = ref<SelectedShow | null>(null);

const selectShow = (showId: string) => {
  const showProgress = refreshingShows.value.get(showId);
  if (showProgress?.show == null) {
    return;
  }
  searchBox.value?.setSelectedShow(showProgress.show);
  refreshingShows.value.delete(showId);
};

const getSubtitles = async (show: SelectedShow) => {
  loadingSubtitles.value = true;
  currentShow.value = show;
  const response = await api.showsShowIdSeasonNumberLanguageGet(
    show.show.id,
    show.season,
    show.language
  );
  episodesWithSubtitles.value = response.episodes || [];
  loadingSubtitles.value = false;
};

const clear = () => {
  episodesWithSubtitles.value = [];
  currentShow.value = null;
};
const formatPercentage = (showId: string) => {
  const showProgress = refreshingShows.value.get(showId)!;

  return (progress: number) => {
    let keyword = "";
    switch (true) {
      case progress == 100:
        return `${showProgress.name}: Ready`;
      case progress < 25:
        keyword = "Fetching seasons of";
        break;
      case progress >= 25:
        keyword = "Fetching subtitles of";
        break;
      default:
        keyword = "Fetching";
        break;
    }
    return `${keyword} ${showProgress.name}: (${progress}%)`;
  };
};

const refreshShow = async (show: ShowDto) => {
  refreshingShows.value.set(show.id, { name: show.name, progress: 0 });
  await sendRefreshAsync(show.id);
};

const needRefresh = async (show: ShowDto) => {
  ElMessage({
    message:
      "We don't have any subtitle for that show. We're fetching them from Addic7ed.\nPlease Try later.",
    type: "warning",
    duration: 5000,
  });
  await refreshShow(show);
};
const progressHandler: ProgressHandler = (progress) => {
  const progressShow = refreshingShows.value.get(progress.showId)!;
  if (progress.progress < progressShow.progress) {
    console.error("Got progress lower than current value");
    return;
  }
  refreshingShows.value.set(progress.showId, {
    ...progressShow,
    progress: progress.progress,
  });
};

const doneHandler: DoneHandler = async (show) => {
  await unsubscribeShowAsync(show.id!);
  const progressShow = refreshingShows.value.get(show.id)!;
  refreshingShows.value.set(show.id, {
    ...progressShow,
    show: show,
  });
  //Auto refresh if the current show is the one that has finished refreshing
  if (currentShow.value?.show.id === show.id) {
    refreshingShows.value.delete(show.id);
    await getSubtitles(currentShow.value);
  }
};
onProgress(progressHandler);
onDone(doneHandler);
onUnmounted(() => {
  offProgress(progressHandler);
  offDone(doneHandler);
});
</script>

<style scoped>
.search-button {
  margin-left: 1em;
}

.progress-bar {
  padding-bottom: 0.5rem;
  flex: 1 0 100%;
}

.progress-container {
  display: flex;
  flex-wrap: wrap;
}

.progress-bar-with-button {
  padding-bottom: 0.5rem;
  flex: 1 0 50%;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>

<template>
  <el-container>
    <el-collapse id="episodes">
      <el-collapse-item
        v-for="episode in props.episodes"
        v-bind:key="episode.number"
        :title="`Ep. ${episode.number}: ${episode.title}`"
        :name="episode.number"
      >
        <el-table table-layout="auto" :data="episode.subtitles">
          <el-table-column label="Version" prop="version"></el-table-column>
          <el-table-column label="Completed" prop="completed">
            <template #default="scope">
              <i v-if="scope.row.completed" class="fa-solid fa-check" />
              <span v-else></span>
            </template>
          </el-table-column>
          <el-table-column label="Hearing Impaired" prop="hearingImpaired">
            <template #default="scope">
              <i
                v-if="scope.row.hearingImpaired"
                class="fa-solid fa-ear-deaf"
              />
              <span v-else></span>
            </template>
          </el-table-column>
          <el-table-column label="HD" prop="hd">
            <template #default="scope">
              <i v-if="scope.row.hd" class="fa-solid fa-check" />
              <span v-else></span>
            </template>
          </el-table-column>
          <el-table-column label="Downloads" prop="downloadCount">
            <template #default="scope">
              <el-button type="primary" @click="downloadSubtitle(scope.row)">
                <i class="fa-solid fa-download fa-fw" />
                {{ scope.row.downloadCount }}
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-collapse-item>
    </el-collapse>
  </el-container>
</template>

<script setup lang="ts">
import { defineProps } from "vue";
import { EpisodeWithSubtitlesDto, SubtitleDto } from "@/api";
import { ElMessage } from "element-plus";

interface Props {
  episodes: Array<EpisodeWithSubtitlesDto>;
}

const props = defineProps<Props>();
const downloadSubtitle = async (sub: SubtitleDto) => {
  ElMessage({
    message: "Subtitle download started ... It might take a moment.",
    type: "success",
  });
  window.location.href = `${process.env.VUE_APP_API_PATH}${sub.downloadUri}`;
  sub.downloadCount!++;
};
</script>
<style scoped>
#episodes {
  flex-grow: 1;
}
</style>

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
              <el-button
                type="primary"
                @click="downloadSubtitle(scope.row)"
                :disabled="currentlyDownloading.has(scope.row.subtitleId)"
              >
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
import { defineProps, ref } from "vue";
import { createWriteStream } from "streamsaver";

import {
  Configuration,
  EpisodeWithSubtitlesDto,
  SubtitleDto,
  SubtitlesApi,
} from "@/api";
import { ElMessage } from "element-plus";

interface Props {
  episodes: Array<EpisodeWithSubtitlesDto>;
}

const api = new SubtitlesApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);
const props = defineProps<Props>();

const currentlyDownloading = ref<Map<string, boolean>>(new Map());
const downloadSubtitle = async (sub: SubtitleDto) => {
  currentlyDownloading.value.set(sub.subtitleId!, true);
  ElMessage({
    message: "Subtitle download started ... It might take a moment.",
    type: "success",
  });
  const response = await api.downloadSubtitle(sub.subtitleId!);
  const header = response.headers.get("Content-Disposition");
  const parts = header!.split(";");
  const filename = parts[1].split("=")[1] ?? "sub.srt";

  const fileStream = createWriteStream(filename);
  const readableStream = response.body;

  // More optimized
  if (window.WritableStream && readableStream?.pipeTo) {
    return readableStream.pipeTo(fileStream).then(() => {
      sub.downloadCount!++;
      currentlyDownloading.value.delete(sub.subtitleId!);
    });
  }

  const writer = fileStream.getWriter();

  const reader = response!.body!.getReader();
  const pump = (): unknown =>
    reader
      .read()
      .then((res) =>
        res.done ? writer.close() : writer.write(res.value).then(pump)
      )
      .then(() => {
        sub.downloadCount!++;
        currentlyDownloading.value.delete(sub.subtitleId!);
      });

  await pump();
};
</script>
<style scoped>
#episodes {
  flex-grow: 1;
}
</style>

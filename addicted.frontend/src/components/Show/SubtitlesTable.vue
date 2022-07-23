<template>
  <el-container>
    <el-collapse id="episodes">
      <el-collapse-item
        v-for="episode in props.episodes"
        v-bind:key="episode.number"
        :title="`Ep. ${episode.number}: ${episode.title}`"
        :name="episode.number"
        style="--el-collapse-header-font-size: 1.2rem"
      >
        <el-table table-layout="auto" :data="episode.subtitles">
          <el-table-column label="Version" prop="version"></el-table-column>
          <el-table-column label="Completed" prop="completed">
            <template #default="scope">
              <Check height="24px" v-if="scope.row.completed" />
              <span v-else></span>
            </template>
          </el-table-column>
          <el-table-column label="Hearing Impaired" prop="hearingImpaired">
            <template #default="scope">
              <HearingOffIcon v-if="scope.row.hearingImpaired" />
              <span v-else></span>
            </template>
          </el-table-column>
          <el-table-column label="HD" prop="hd">
            <template #default="scope">
              <Check height="24px" v-if="scope.row.hd" />
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
                <Download height="24px" />
                {{ scope.row.downloadCount }}
              </el-button>
              <el-progress
                v-show="currentlyDownloading.has(scope.row.subtitleId)"
                :percentage="100"
                status="success"
                :indeterminate="true"
                :duration="5"
              />
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
import streamSaver from "streamsaver";
import HearingOffIcon from "vue-material-design-icons/EarHearingOff.vue";

import { EpisodeWithSubtitlesDto, SubtitleDto } from "~/api/api";
import { ElMessage } from "element-plus";
import { Download, Check } from "@element-plus/icons-vue";
import { api } from "~/composables/rest/api";
import { mevent } from "~/composables/matomo/tracking";

interface Props {
  episodes: Array<EpisodeWithSubtitlesDto>;
}

const props = defineProps<Props>();
// eslint-disable-next-line no-import-assign,@typescript-eslint/no-unused-vars
streamSaver.mitm = "/mitm.html";

const currentlyDownloading = ref<Map<string, boolean>>(new Map());
const downloadSubtitle = async (sub: SubtitleDto) => {
  mevent("download-subtitle", { subtitle: sub });
  currentlyDownloading.value.set(sub.subtitleId!, true);
  ElMessage({
    message: "Subtitle download started ... It might take a moment.",
    type: "success",
  });
  const response = await api.subtitles.downloadSubtitle(sub.subtitleId!);
  const header = response.headers.get("Content-Disposition");
  const parts = header!.split(";");
  const filename = parts[1].split("=")[1] ?? "sub.srt";

  const fileStream = createWriteStream(filename);
  const readableStream = response.body;

  const UpdateDownloaded = () => {
    sub.downloadCount!++;
    currentlyDownloading.value.delete(sub.subtitleId!);
  };

  // More optimized
  if (window.WritableStream && readableStream?.pipeTo) {
    return readableStream.pipeTo(fileStream).then(UpdateDownloaded);
  }

  const writer = fileStream.getWriter();

  const reader = response!.body!.getReader();
  const pump = (): unknown =>
    reader
      .read()
      .then((res) =>
        res.done ? writer.close() : writer.write(res.value).then(pump)
      )
      .then(UpdateDownloaded);

  await pump();
};
</script>
<style scoped>
#episodes {
  flex-grow: 1;
}
</style>

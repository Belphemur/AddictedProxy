<template>
  <el-row>
    <el-col :offset="2" :span="20">
      <el-card header="Top 10 shows by Downloads">
        <PopularityChart :top="topShows"></PopularityChart>
      </el-card>
    </el-col>
  </el-row>
</template>

<script lang="ts" setup>
import PopularityChart from "~/components/Stats/PopularityChart.vue";
import { api } from "~/composables/rest/api";

const topShows = (await api.stats.downloadsDetail(10)).data;
</script>

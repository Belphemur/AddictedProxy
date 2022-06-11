<template>
  <el-row>
    <el-col :offset="2" :span="20">
      <el-card header="Top 10 shows by Popularity">
        <PopularityChart :top="topShows"></PopularityChart>
      </el-card>
    </el-col>
  </el-row>
</template>

<script lang="ts" setup>
import { Configuration, StatsApi } from "@/api";
import PopularityChart from "@/components/Stats/PopularityChart.vue";

const api = new StatsApi(
  new Configuration({ basePath: process.env.VUE_APP_API_PATH })
);
const topShows = await api.statsTopTopGet(10);
</script>

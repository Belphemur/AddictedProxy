<template>
  <Suspense>
    <ShowsPopularity />
  </Suspense>
  <el-divider>
    <el-icon>
      <Download />
    </el-icon>
  </el-divider>
  <Suspense>
    <ShowsDownloads />
  </Suspense>
</template>

<script lang="ts" setup>
import { Download } from "@element-plus/icons-vue";
</script>

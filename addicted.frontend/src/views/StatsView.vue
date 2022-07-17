<template>
  <Suspense>
    <ShowsPopularity />
    <template #fallback>
      <el-skeleton :rows="5" animated :loading="true" />
    </template>
  </Suspense>
  <el-divider>
    <el-icon>
      <Download />
    </el-icon>
  </el-divider>
  <Suspense>
    <ShowsDownloads />
    <template #fallback>
      <el-skeleton :rows="5" animated :loading="true" />
    </template>
  </Suspense>
</template>

<script lang="ts" setup>
import { Download } from "@element-plus/icons-vue";
</script>

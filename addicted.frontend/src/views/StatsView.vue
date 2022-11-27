<template>
  <Suspense>
    <ShowsDownloads />
    <template #fallback>
      <el-skeleton :rows="5" animated :loading="true" />
    </template>
  </Suspense>
</template>

<script lang="ts" setup>
</script>

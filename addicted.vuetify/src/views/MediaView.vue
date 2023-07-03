<script setup lang="ts">

import MediaDetailLoader from "@/components/media/MediaDetailLoader.vue";
</script>

<template>
  <v-row justify="center">
    <v-col cols="6">
      <suspense>
        <template #default>
          <media-detail-loader/>
        </template>
        <template #fallback>
          <v-progress-linear indeterminate></v-progress-linear>
        </template>
      </suspense>
    </v-col>
  </v-row>
</template>

<style scoped>

</style>

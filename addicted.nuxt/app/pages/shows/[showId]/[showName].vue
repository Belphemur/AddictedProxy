<script setup lang="ts">
import MediaDetailView from "~/components/media/MediaDetailView.vue";

const route = useRoute();
const showId = route.params.showId.toString();

definePageMeta({
  name: "show-details",
})
</script>

<template>
  <v-container fluid class="pa-2 pa-sm-4" style="max-width: 1600px">
    <suspense>
      <template #default>
        <media-detail-view :show-id="showId"></media-detail-view>
      </template>
      <template #fallback>
        <v-progress-linear indeterminate></v-progress-linear>
      </template>
    </suspense>
  </v-container>
</template>

<style scoped>

</style>

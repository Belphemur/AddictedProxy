<script setup lang="ts">
import MediaDetailView from "~/components/media/MediaDetailView.vue";

const route = useRoute();
const showId = route.params.showId.toString();

definePageMeta({
  name: "show-details",
})
</script>

<template>
    <v-col cols="12" lg="8" offset-lg="2" >
      <suspense>
        <template #default>
          <media-detail-view :show-id="showId"></media-detail-view>
        </template>
        <template #fallback>
          <v-progress-linear indeterminate></v-progress-linear>
        </template>
      </suspense>
    </v-col>
</template>

<style scoped>

</style>

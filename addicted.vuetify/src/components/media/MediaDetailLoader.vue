<script setup lang="ts">
import {useRouter} from "vue-router";
import {api} from "@/composables/rest/api";
import MediaDetails from "@/components/media/MediaDetails.vue";

const router = useRouter();
const showId = router.currentRoute.value.params.showId;
const response = await api.media.mediaDetails(`${showId}`);
const mediaInfo = response.data;
</script>

<template>
  <media-details :details="mediaInfo"/>
</template>

<style scoped>

</style>

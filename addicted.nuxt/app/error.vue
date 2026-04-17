<script setup lang="ts">
import type { NuxtError } from '#app'
import DefaultBar from '@/layouts/default/AppBar.vue';
import { usePageLayout } from "~/composables/usePageLayout";
import { mdiArrowLeft, mdiHome } from "@mdi/js";

const props = defineProps({
  error: Object as () => NuxtError,
});
const layout = usePageLayout();

// If the error is on a season URL, offer a link back to the show page.
const showLink = computed(() => {
  const path = props.error?.url ?? '';
  const match = path.match(/^\/shows\/([^/]+)\/([^/]+)\/\d+$/);
  if (!match) return null;
  return `/shows/${match[1]}/${match[2]}`;
});

const handleGoToShow = () => clearError({ redirect: showLink.value! });
const handleGoHome = () => clearError({ redirect: '/' });
</script>

<template>
  <v-app :style="{ background: 'none' }">
    <div class="bg-image"></div>
    <default-bar />
    <v-main style="padding-top: 3rem">
      <v-container fluid :class="layout.classes.pageContainer" :style="{ maxWidth: layout.maxWidth }">
        <v-sheet rounded="lg" :color="layout.colors.primaryPanel" :class="layout.classes.primaryPanel">
          <div class="text-center">
            <h1 class="text-h3 font-weight-bold mb-2">{{ error?.statusCode }}</h1>
            <p class="text-body-1 text-medium-emphasis mb-6">
              {{ error?.statusMessage || "Page not found" }}
            </p>
            <div class="d-flex justify-center ga-3">
              <v-btn v-if="showLink" :prepend-icon="mdiArrowLeft" color="primary" @click="handleGoToShow">
                Go back to show
              </v-btn>
              <v-btn :prepend-icon="mdiHome" :color="showLink ? undefined : 'primary'"
                :variant="showLink ? 'outlined' : 'flat'" @click="handleGoHome">
                Home
              </v-btn>
            </div>
          </div>
        </v-sheet>
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
.bg-image {
  filter: blur(8px);
  -webkit-filter: blur(8px);
  height: 100vh;
  width: 100%;
  z-index: -1;
  position: fixed;
  left: 0;
  right: 0;
  display: block;
  background-position: center;
  background-repeat: no-repeat;
  background-size: cover;
  background-image: url('/img/background-small.webp');
}
</style>

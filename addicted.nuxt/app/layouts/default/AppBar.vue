<template>
  <v-app-bar dense elevation="4" shaped>
    <v-app-bar-nav-icon variant="text" @click.stop='emit("drawerClicked")' v-if="isMobile"></v-app-bar-nav-icon>
    <template v-else>
      <NuxtLink v-for="route in routes" :key="route.path" :to="route.path" custom v-slot="{ navigate, isActive }">
        <v-app-bar-nav-icon :active="isActive" :text="route.name" @click="navigate">
          <v-icon>{{ route.meta.icon }}</v-icon>
          <v-tooltip activator="parent" location="bottom">{{ route.name }}
          </v-tooltip>
        </v-app-bar-nav-icon>
      </NuxtLink>
    </template>
    <v-app-bar-title>
      <v-icon :icon="logo" />
      Gestdown: Subtitle Aggregator
    </v-app-bar-title>

  </v-app-bar>
</template>

<script lang="ts" setup>
import logo from "@/components/icon/logo.vue";
import { useRouter } from "vue-router";
import { orderBy, filter } from "lodash-es";

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "drawerClicked"): void;
}>();

export interface Props {
  isMobile: boolean;
}

const { isMobile } = defineProps<Props>();

const router = useRouter();
const routes = orderBy(
  filter(router.getRoutes(), (route) => {
    return route.meta.order != undefined;
  }),
  ["meta.order"],
  ["asc"]
);

</script>

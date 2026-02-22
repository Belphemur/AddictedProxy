<template>
  <v-app-bar density="compact" elevation="2" color="rgba(0,0,0,0.7)">
    <v-app-bar-title class="text-body-1 flex-grow-0">
      <v-icon :icon="logo" size="small" />
      Gestdown
    </v-app-bar-title>

    <v-spacer />
    <NuxtLink v-for="route in routes" :key="route.path" :to="route.path" custom v-slot="{ navigate, isActive }">
      <v-btn :active="isActive" @click="navigate" variant="text" size="small">
        <v-icon size="small" start>{{ route.meta.icon }}</v-icon>
        {{ route.name }}
      </v-btn>
    </NuxtLink>
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

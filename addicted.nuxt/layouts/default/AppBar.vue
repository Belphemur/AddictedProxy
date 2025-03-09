<template>
  <v-app-bar dense
             elevation="4"
             shaped>
    <v-app-bar-nav-icon variant="text" @click.stop='emit("drawerClicked")' v-if="isMobile"></v-app-bar-nav-icon>
    <template v-else>
      <v-app-bar-nav-icon
          v-for="route in routes"
          icon="true"
          :active="route.path == router.currentRoute.value.fullPath"
          :text="route.name"
          v-on:click="router.push(route)">
        <v-icon >{{route.meta.icon}}</v-icon>
        <v-tooltip
            activator="parent"
            location="bottom"
        >{{ route.name }}
        </v-tooltip>
      </v-app-bar-nav-icon>
    </template>
    <v-app-bar-title>
      <v-icon :icon="logo"/>
      Gestdown: Addic7ed Proxy
    </v-app-bar-title>

  </v-app-bar>
</template>

<script lang="ts" setup>
import logo from "@/components/icon/logo.vue";
import {useRouter} from "vue-router";
// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "drawerClicked"): void;
}>();

export interface Props {
  isMobile: boolean;
}

const {isMobile} = defineProps<Props>();

const router = useRouter();
const routes = useOrderBy(useFilter(router.getRoutes(), (route) => {
  return route.meta.order != undefined;
}), ["meta.order"], ["asc"]);

</script>

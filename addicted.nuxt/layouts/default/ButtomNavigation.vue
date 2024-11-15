<script setup lang="ts">
import {useRouter} from "vue-router";

const router = useRouter();
const routes = useOrderBy(useFilter(router.getRoutes(), (route) => {
  return route.meta.order != undefined;
}), ["meta.order"], ["asc"]);

const drawer = defineModel<boolean>()


</script>

<template>
  <v-bottom-navigation
      :active="drawer"
      color="teal"
      grow
      :model-value="router.currentRoute.value"
  >
    <v-btn v-for="route in routes"
           v-bind:key="route.path"
           :value="route"
           v-on:click="router.push(route)">
      <v-icon>{{route.meta.icon}}</v-icon>

      {{ route.name }}
    </v-btn>
  </v-bottom-navigation>
</template>

<style scoped>

</style>
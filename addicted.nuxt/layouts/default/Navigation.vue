<script setup lang="ts">
import {useRouter} from "vue-router";

export interface Props {
  mobile: boolean;
}

const router = useRouter();
const routes = useOrderBy(useFilter(router.getRoutes(), (route) => {
  return route.meta.order != undefined;
}), ["meta.order"], ["asc"]);

const drawer = defineModel<boolean>()

const props = defineProps<Props>()

</script>

<template>
  <v-navigation-drawer
      :expand-on-hover="!props.mobile"
      :rail="!props.mobile"
      v-model="drawer"
      :location="props.mobile ? 'bottom': 'left'"
      :permanent="!props.mobile"
  >
    <v-list>
      <v-list density="compact" nav>
        <v-list-item v-for="route in routes"
                     v-bind:key="route.path"
                     :route="route"
                     :title="route.name"
                     :prepend-icon="route.meta.icon"
                     :to="route.path"
                     :active="route.path == router.currentRoute.value.fullPath"
                     :value="route">
        </v-list-item>
      </v-list>
    </v-list>
  </v-navigation-drawer>
</template>

<style scoped>

</style>

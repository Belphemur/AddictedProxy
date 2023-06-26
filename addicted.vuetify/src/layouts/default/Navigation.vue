<script setup lang="ts">
import {useRouter} from "vue-router";
import {defineModel} from "vue";
import * as _ from "lodash-es";
import {useDisplay} from "vuetify";

const router = useRouter();
const routes = _.orderBy(router.getRoutes(), ["meta.order"], ["asc"]);

const drawer = defineModel<boolean>()
const {mobile} = useDisplay()

</script>

<template>
  <v-navigation-drawer
    :expand-on-hover="!mobile"
    :rail="!mobile"
    v-model="drawer"
    :location="mobile ? 'bottom': 'left'">
    <v-list>
      <v-list density="compact" nav>
        <v-list-item prepend-icon="mdi-home-city" title="Home" value="home"></v-list-item>
        <v-list-item prepend-icon="mdi-account" title="My Account" value="account"></v-list-item>
        <v-list-item prepend-icon="mdi-account-group-outline" title="Users" value="users"></v-list-item>
      </v-list>
    </v-list>
  </v-navigation-drawer>
</template>

<style scoped>

</style>

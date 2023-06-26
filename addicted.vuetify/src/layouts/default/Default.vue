<template>
  <v-app>
    <default-bar v-on:drawer-clicked="drawer = !drawer"/>

    <navigation v-model="drawer" />

    <default-view />
  </v-app>
</template>

<script lang="ts" setup>
  import DefaultBar from './AppBar.vue'
  import DefaultView from './View.vue'
  import Navigation from "@/layouts/default/Navigation.vue";
  import {ref} from "vue";
  const drawer = ref(true)
</script>

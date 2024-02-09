<template>
  <v-app :style="defaultBackground">
    <div class="bg-image" :style="backgroundStyles"></div>
    <default-bar v-on:drawer-clicked="drawer = !drawer"/>

    <navigation v-model="drawer" :mobile="isMobile"/>

    <default-view/>
  </v-app>
</template>

<script lang="ts" setup>
import DefaultBar from '@/layouts/default/AppBar.vue';
import DefaultView from '@/layouts/default/View.vue';
import Navigation from "@/layouts/default/Navigation.vue";
import {ref} from "vue";

const {isMobile} = useDevice();

const drawer = ref<boolean>(!isMobile)

const backgroundStyles = {backgroundImage: `url('/img/background.webp')`}

const defaultBackground = {
  background: "none",
}
</script>
<style scoped>
.bg-image {
  /* Add the blur effect */
  filter: blur(8px);
  -webkit-filter: blur(8px);

  /* Full height */
  height: 100vh;
  width: 100%;
  z-index: -1;
  position: fixed;
  left: 0;
  right: 0;
  display: block;

  /* Center and scale the image nicely */
  background-position: center;
  background-repeat: no-repeat;
  background-size: cover;
}
</style>

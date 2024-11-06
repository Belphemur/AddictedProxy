<template>
  <v-app :style="defaultBackground">
    <div class="bg-image" ></div>
    <default-bar v-on:drawer-clicked="drawer = !drawer" :is-mobile="isMobile"/>

    <ButtomNavigation v-model="drawer" v-if="isMobile"/>

    <default-view/>
  </v-app>
</template>

<script lang="ts" setup>
import DefaultBar from '@/layouts/default/AppBar.vue';
import DefaultView from '@/layouts/default/View.vue';
import {ref} from "vue";
import ButtomNavigation from "~/layouts/default/ButtomNavigation.vue";

const {isMobile} = useDevice();

const drawer = ref<boolean>(isMobile)

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
  background-image: url('/img/background-small.webp')}
</style>

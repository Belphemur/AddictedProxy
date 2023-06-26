<template>
  <v-app :style="defaultBackground">
    <div class="bg-image" :style="backgroundStyle"></div>
    <default-bar v-on:drawer-clicked="drawer = !drawer"/>

    <navigation v-model="drawer"/>

    <default-view />
  </v-app>
</template>

<script lang="ts" setup>
import DefaultBar from './AppBar.vue'
import DefaultView from './View.vue'
import Navigation from "@/layouts/default/Navigation.vue";
import {ref} from "vue";
import background from "@/assets/background.webp";
import {useDisplay} from "vuetify";

const {mobile} = useDisplay()

const drawer = ref<boolean>(!mobile.value)


const backgroundStyle = {
  backgroundImage: `url(${background})`,
};
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

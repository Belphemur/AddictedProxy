<template>
  <el-container>
    <div class="bg-image" :style="backgroundStyle"></div>
    <BaseSide />
    <el-main>
      <router-view />
    </el-main>
  </el-container>
</template>
<script setup lang="ts">
import background from "~/assets/background.webp";

const backgroundStyle = {
  backgroundImage: `url(${background})`,
};
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

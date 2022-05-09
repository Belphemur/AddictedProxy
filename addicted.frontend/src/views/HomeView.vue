<template>
  <el-container id="container">
    <div id="search">
      <SearchComponent />
    </div>
  </el-container>
</template>

<script setup lang="ts"></script>

<style scoped>
#search {
  width: 50%;
  display: flex;
}
#container {
  align-content: center;
  justify-content: center;
}
</style>

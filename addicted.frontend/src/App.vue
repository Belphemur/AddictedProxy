<template>
  <el-container>
    <BaseSide />
    <el-main>
      <router-view />
    </el-main>
  </el-container>
</template>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: var(--el-text-color-primary);
  height: 100%;
}
</style>

<template>
  <BaseHeader />
  <div style="display: flex">
    <BaseSide />
    <div style="width: 100%">
      <router-view />
    </div>
  </div>
</template>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: var(--el-text-color-primary);
}
</style>

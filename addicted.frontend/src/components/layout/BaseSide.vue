<template>
  <el-affix>
    <el-aside width="200px" class="menu">
      <el-scrollbar>
        <el-menu :collapse="false" :router="true" :default-active="$route.path">
          <img
            :src="isDark ? transparent : black"
            alt="gestdown logo"
            width="180"
          />
          <el-menu-item-group>
            <el-menu-item
              v-for="route in $router.getRoutes()"
              v-bind:key="route.path"
              :route="route"
              :index="route.path"
            >
              <el-icon v-if="route.meta.icon != null">
                <component :is="route.meta.icon"></component>
              </el-icon>
              {{ route.name }}
            </el-menu-item>
          </el-menu-item-group>
          <el-menu-item h="full">
            <el-switch
              v-model="isDark"
              size="large"
              :active-icon="Moon"
              :inactive-icon="Sunny"
            />
          </el-menu-item>
        </el-menu>
      </el-scrollbar>
    </el-aside>
  </el-affix>
</template>

<script lang="ts" setup>
import transparent from "@/assets/Logos/Gestdown-logos_transparent.png";
import black from "@/assets/Logos/Gestdown-logos_black.png";
import { isDark } from "@/composables/theme";
import { Sunny, Moon } from "@element-plus/icons-vue";
</script>

<style scoped>
.menu {
  background-color: var(--el-bg-color-overlay);
}
</style>

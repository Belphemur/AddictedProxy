<template>
  <el-aside width="200px">
    <el-scrollbar>
      <el-menu :default-openeds="['1', '1']">
        <el-sub-menu index="1">
          <template #title>
            <el-icon>
              <Setting />
            </el-icon>
            <span>Menu</span>
          </template>
          <el-menu-item-group>
            <router-link to="/">
              <el-menu-item index="1-1">Home</el-menu-item>
            </router-link>

            <router-link to="/about">
              <el-menu-item index="1-2">About</el-menu-item>
            </router-link>
          </el-menu-item-group>
        </el-sub-menu>
      </el-menu>
    </el-scrollbar>
  </el-aside>
</template>

<script lang="ts" setup>
import { Setting } from "@element-plus/icons-vue";
</script>

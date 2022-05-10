<template>
  <el-aside width="200px">
    <el-scrollbar>
      <el-menu :default-openeds="['1']" :collapse="false">
        <img
          :src="isDark ? transparent : black"
          alt="gestdown logo"
          width="180"
        />
        <el-menu-item-group>
          <router-link to="/">
            <el-menu-item index="1-1">Home</el-menu-item>
          </router-link>

          <router-link to="/about">
            <el-menu-item index="1-2">About</el-menu-item>
          </router-link>
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
</template>

<script lang="ts" setup>
import transparent from "@/assets/Logos/Gestdown-logos_transparent.png";
import black from "@/assets/Logos/Gestdown-logos_black.png";
import { isDark } from "@/composables";
import { Sunny, Moon } from "@element-plus/icons-vue";
</script>

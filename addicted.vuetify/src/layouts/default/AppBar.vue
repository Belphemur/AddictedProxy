<template>
  <v-app-bar flat>
    <v-app-bar-nav-icon variant="text" @click.stop='emit("drawerClicked")'></v-app-bar-nav-icon>
    <v-app-bar-title>
      <v-icon icon="mdi-circle-slice-6" />

      Gestdown: Addic7ed Proxy
    </v-app-bar-title>
  </v-app-bar>
</template>

<script lang="ts" setup>
// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "drawerClicked"): void;
}>();
</script>

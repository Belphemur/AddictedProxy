<template>
  <v-container>
    <v-row justify="center" align-content="center">
      <v-col cols="10" align-self="center" class="text-center">
        <v-sheet rounded>
          <v-row>
            <v-col>
              <v-icon size="100" color="primary" class="mb-4">{{mdiCodeBraces}}</v-icon>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              The api documentation has moved to <a href="https://gestdown.readme.io/" target="_blank">Readme</a>
            </v-col>
          </v-row>
        </v-sheet>
      </v-col>
    </v-row>
  </v-container>
</template>

<script type="ts" setup>

import {mdiCodeBraces} from "@mdi/js";

definePageMeta({
  order: 20,
  icon: mdiCodeBraces,
  name: "Api"
})
useSeoMeta({
  title: "Gestdown: API Documentation",
  description: "API to help you search for subtitle for different show available on Addic7edn",
  ogDescription: "API to help you search for subtitle for different show available on Addic7ed",
  ogImage: "/img/logo.png"
})

if (process.client) {
  window.open("https://gestdown.readme.io/", "_blank")
}
</script>

<style scoped>

</style>

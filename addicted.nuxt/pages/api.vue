<template>
  <v-row justify="center">
    <v-col cols="10" align-self="center">
      <v-card id="api-view">
        <div class="swagger" id="swagger"></div>
      </v-card>
    </v-col>
  </v-row>
</template>

<script type="ts" setup>
import {SwaggerUIBundle} from "swagger-ui-dist";
import "swagger-ui-dist/swagger-ui.css";
import {onMounted} from "vue";
import {getApiServerUrl} from "~/composables/rest/api";

definePageMeta({
  order: 20,
  icon: "mdi-code-braces",
  name: "Api"
})
useSeoMeta({
  title: "Gestdown: API Documentation",
  description: "API to help you search for subtitle for different show available on Addic7edn",
  ogDescription: "API to help you search for subtitle for different show available on Addic7ed",
  ogImage: "/img/logo.png"
})

const config = useRuntimeConfig();

onMounted(() => {
  SwaggerUIBundle({
    url: getApiServerUrl() + "/api/v1/swagger.json",
    dom_id: "#swagger"
  });
});
</script>

<style scoped>
#api-view {
  text-align: left;
}
</style>
<style>
.swagger-ui {
  filter: invert(88%) hue-rotate(180deg);
}

.swagger-ui .highlight-code {
  filter: invert(100%) hue-rotate(180deg);
}

.swagger-ui input {
  color: black;
}

.swagger-ui input::placeholder {
  color: gray;
}

.swagger-ui input[disabled]::placeholder {
  color: black;
}
</style>

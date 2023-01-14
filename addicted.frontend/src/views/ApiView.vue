<template>
  <el-card id="api-view">
    <div class="swagger" id="swagger"></div>
  </el-card>
</template>

<script type="ts" setup>
import { SwaggerUIBundle } from "swagger-ui-dist";
import "swagger-ui-dist/swagger-ui.css";
import { onMounted } from "vue";

onMounted(() => {
  SwaggerUIBundle({
    url: import.meta.env.VITE_APP_API_PATH + "/api/v1/swagger.json",
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
html.dark .swagger-ui {
  filter: invert(88%) hue-rotate(180deg);
}

html.dark .swagger-ui .highlight-code {
  filter: invert(100%) hue-rotate(180deg);
}

html.dark .swagger-ui input {
  color: black;
}

html.dark .swagger-ui input::placeholder {
  color: gray;
}

html.dark .swagger-ui input[disabled]::placeholder {
  color: black;
}
</style>

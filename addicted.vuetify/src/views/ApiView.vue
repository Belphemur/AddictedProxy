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

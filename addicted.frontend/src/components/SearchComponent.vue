<template>
  <div class="mt-4">
    <el-autocomplete
      v-model="searchInput"
      :fetch-suggestions="querySearch"
      clearable
      placeholder="Search for your episode: Wellington S01E01"
      class="inline-input"
      size="large"
      :debounce="500"
      :trigger-on-focus="false"
    >
      <template #prepend>
        <el-select
          v-model="languageSelect"
          placeholder="Select"
          style="width: 115px"
        >
          <el-option label="English" value="en" />
        </el-select>
      </template>
    </el-autocomplete>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";

const searchInput = ref("");
const userLang = (navigator.language || navigator.userLanguage).split("-")[0];
const languageSelect = ref(userLang);

const querySearch = (query: string, cb: (param: unknown) => void) => {
  console.log(query);
  cb([{ value: "Hello World" }]);
};
</script>

<style scoped></style>

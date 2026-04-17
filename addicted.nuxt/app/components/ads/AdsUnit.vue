<template>
  <ScriptGoogleAdsense v-if="adsEnabled" v-bind="$attrs" />
</template>

<script lang="ts" setup>
defineOptions({ inheritAttrs: false });

const { ads } = useRuntimeConfig().public;
const adsEnabled = ads.enabled;
</script>

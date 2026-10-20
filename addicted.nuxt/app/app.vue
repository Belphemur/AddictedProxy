<template>
  <NuxtLayout/>
</template>
<script setup lang="ts">
import { useSiteBrand } from "~/composables/useSiteBrand";

const { siteName } = useSiteBrand();
useHead({
  htmlAttrs: {
    lang: "en-US",
  }
})
useSeoMeta(() => ({
  ogType: "website",
  ogImage: "/img/Gestdown-logos.jpeg",
  twitterCard: "summary",
  twitterImage: "/img/Gestdown-logos.jpeg",
  ogLocale: "en_US",
  ogSiteName: `${siteName}: Search and download subtitles from Addic7ed`,
}))

</script>

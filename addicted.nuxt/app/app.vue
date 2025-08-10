<template>
  <NuxtLayout/>
</template>
<script lang="ts" setup>
useHead({
  htmlAttrs: {
    lang: "en-US",
  }
})
useSeoMeta({
  ogType: "website",
  ogImage: "/img/Gestdown-logos.jpeg",
  twitterCard: "summary",
  twitterImage: "/img/Gestdown-logos.jpeg",
  ogLocale: "en_US",
  ogSiteName: "Gestdown: Search and download subtitles from Addic7ed",
})

</script>

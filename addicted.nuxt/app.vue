<template>
  <NuxtLayout/>
</template>
<script setup lang="ts">
const config = useRuntimeConfig();
useHead({
  script: [{
    innerHTML: "const _mtm = window._mtm = window._mtm || [];\n" +
        "        _mtm.push({'mtm.startTime': (new Date().getTime()), 'event': 'mtm.Start'});\n" +
        "        const d=document, g=d.createElement('script'), s=d.getElementsByTagName('script')[0];\n" +
        `        g.async=true; g.src='${config.public.VITE_APP_MATOMO}'; s.parentNode.insertBefore(g,s);`
  }]
})
</script>

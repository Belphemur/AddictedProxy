<script setup lang="ts">
definePageMeta({
    name: "show-season",
    validate(route) {
        const s = route.params.seasonNumber;
        return typeof s === "string" && /^\d+$/.test(s) && parseInt(s, 10) > 0;
    },
});

const route = useRoute();
const getSingleRouteParam = (value: string | string[]) => Array.isArray(value) ? value[0] ?? "" : value;

await navigateTo(
    `/shows/${getSingleRouteParam(route.params.showId)}/${getSingleRouteParam(route.params.showName)}`,
    { redirectCode: 301 },
);
</script>

<template></template>

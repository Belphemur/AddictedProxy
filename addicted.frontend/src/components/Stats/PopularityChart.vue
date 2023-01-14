<template>
  <PieChart
    :labels="showName"
    :data="showPopularity"
    :backgroundColors="showColor"
  ></PieChart>
</template>

<script setup lang="ts">
import { TopShowDto } from "~/api/api";
import { computed, defineProps } from "vue";
import PieChart from "~/components/Charts/PieChart.vue";
import { stringToColour } from "~/components/Charts/generateColorFromString";
import { ChartData } from "chart.js";

interface Props {
  top: Array<TopShowDto>;
}

const props = defineProps<Props>();

const showName = computed(() => {
  return props.top.map((value) => value.show?.name);
});
const showPopularity = computed(() => {
  return props.top.map((value) => value.popularity) as unknown as ChartData;
});
const showColor = computed(() => {
  return props.top.map((value) => stringToColour(value.show?.name ?? "N/A"));
});
</script>

<template>
  <div class="container">
    <Pie :data="data" :options="options" />
  </div>
</template>
<script setup lang="ts">
import { defineProps } from "vue";

import { Pie } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  ChartData,
} from "chart.js";

interface Props {
  labels: string[];
  backgroundColors: string[];
  data: ChartData;
}

ChartJS.register(Title, Tooltip, Legend, ArcElement, CategoryScale);

const props = defineProps<Props>();
const data = {
  labels: props.labels,
  datasets: [
    {
      data: props.data,
      backgroundColor: props.backgroundColors,
    },
  ],
};

const options = {
  responsive: true,
  maintainAspectRatio: false,
};
</script>

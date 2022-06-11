import { defineComponent, h, PropType } from "vue";

import { Pie } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  Plugin,
} from "chart.js";

ChartJS.register(Title, Tooltip, Legend, ArcElement, CategoryScale);

// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore
export default defineComponent({
  name: "PieChart",
  components: {
    Pie,
  },
  props: {
    chartId: {
      type: String,
      default: "pie-chart",
    },
    width: {
      type: Number,
      default: 400,
    },
    height: {
      type: Number,
      default: 400,
    },
    cssClasses: {
      default: "",
      type: String,
    },
    styles: {
      type: Object as PropType<Partial<CSSStyleDeclaration>>,
      // eslint-disable-next-line @typescript-eslint/no-empty-function
      default: () => {},
    },
    plugins: {
      type: Array as PropType<Plugin<"pie">[]>,
      default: () => [],
    },
    labels: {
      type: Array as PropType<string[]>,
      default: () => [],
    },
    data: {
      type: Array,
      default: () => [],
    },
    backgroundColors: {
      type: Array as PropType<string[]>,
      default: () => [],
    },
  },
  setup(props) {
    const chartData = {
      labels: props.labels,
      datasets: [
        {
          data: props.data,
          backgroundColor: props.backgroundColors,
        },
      ],
    };

    const chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
    };

    return () =>
      // eslint-disable-next-line @typescript-eslint/ban-ts-comment
      // @ts-ignore
      h(Pie, {
        chartData,
        chartOptions,
        chartId: props.chartId,
        width: props.width,
        height: props.height,
        cssClasses: props.cssClasses,
        styles: props.styles,
        plugins: props.plugins,
      });
  },
});

import { createApp } from "vue";
import App from "./App.vue";
import "./registerServiceWorker";
import router from "./router";

import "@/styles/index.scss";

import { library, dom } from "@fortawesome/fontawesome-svg-core";
import "element-plus/theme-chalk/dark/css-vars.css";

import { fas } from "@fortawesome/free-solid-svg-icons";
library.add(fas);
import { fab } from "@fortawesome/free-brands-svg-icons";
library.add(fab);
import { far } from "@fortawesome/free-regular-svg-icons";
library.add(far);
dom.watch();

import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

createApp(App)
  .use(router)
  .component("font-awesome-icon", FontAwesomeIcon)
  .mount("#app");

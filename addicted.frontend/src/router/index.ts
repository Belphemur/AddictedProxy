import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import HomeView from "../views/HomeView.vue";
import { Search, Collection } from "@element-plus/icons-vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Home",
    component: HomeView,
    meta: {
      icon: Search,
    },
  },
  {
    path: "/Api",
    name: "Api",
    meta: {
      icon: Collection,
    },
    // route level code-splitting
    // this generates a separate chunk (api.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "api" */ "../views/ApiView.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;

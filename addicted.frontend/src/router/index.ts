import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import HomeView from "../views/HomeView.vue";
import { Search, Collection, DataLine } from "@element-plus/icons-vue";
import { mevent } from "~/composables/matomo/tracking";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Home",
    component: HomeView,
    meta: {
      order: 10,
      icon: Search,
      title: "Gestdown: Addic7ed Proxy",
      metaTags: [
        {
          name: "description",
          content:
            "Help you search for subtitle for different show available on Addic7ed",
        },
        {
          property: "og:description",
          content:
            "Help you search for subtitle for different show available on Addic7ed",
        },
      ],
    },
  },
  {
    path: "/Api",
    name: "Api",
    meta: {
      order: 20,
      icon: Collection,
      title: "Gestdown: API Documentation",
      metaTags: [
        {
          name: "description",
          content:
            "API to help you search for subtitle for different show available on Addic7ed",
        },
        {
          property: "og:description",
          content:
            "API to help you search for subtitle for different show available on Addic7ed",
        },
      ],
    },
    // route level code-splitting
    // this generates a separate chunk (api.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "api" */ "../views/ApiView.vue"),
  },
  {
    path: "/Stats/Top/10",
    name: "Top 10 Shows",
    meta: {
      order: 30,
      icon: DataLine,
      title: "Gestdown: Top 10 shows",
      metaTags: [
        {
          name: "description",
          content: "Top 10 shows by popularity",
        },
        {
          property: "og:description",
          content: "Top 10 shows by popularity",
        },
      ],
    },
    // route level code-splitting
    // this generates a separate chunk (api.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "stats" */ "../views/StatsView.vue"),
  },
];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});

// This callback runs before every route change, including on page load.
router.beforeEach((to, from, next) => {
  // This goes through the matched routes from last to first, finding the closest route with a title.
  // e.g., if we have `/some/deep/nested/route` and `/some`, `/deep`, and `/nested` have titles,
  // `/nested`'s will be chosen.
  const nearestWithTitle = to.matched
    .slice()
    .reverse()
    .find((r) => r.meta && r.meta.title);

  // Find the nearest route element with meta tags.
  const nearestWithMeta = to.matched
    .slice()
    .reverse()
    .find((r) => r.meta && r.meta.metaTags);

  const previousNearestWithMeta = from.matched
    .slice()
    .reverse()
    .find((r) => r.meta && r.meta.metaTags);

  // If a route with a title was found, set the document (page) title to that value.
  if (nearestWithTitle) {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    document.title = nearestWithTitle.meta.title;
  } else if (previousNearestWithMeta) {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    document.title = previousNearestWithMeta.meta.title;
  }

  // Remove any stale meta tags from the document using the key attribute we set below.
  Array.from(document.querySelectorAll("[data-vue-router-controlled]")).map(
    (el) => el.parentNode?.removeChild(el)
  );

  // Skip rendering meta tags if there are none.
  if (!nearestWithMeta) return next();

  // Turn the meta tag definitions into actual elements in the head.
  // eslint-disable-next-line @typescript-eslint/ban-ts-comment
  // @ts-ignore
  nearestWithMeta.meta.metaTags
    .map((tagDef: any) => {
      const tag = document.createElement("meta");

      Object.keys(tagDef).forEach((key) => {
        tag.setAttribute(key, tagDef[key]);
      });

      // We use this to track which meta tags we create so we don't interfere with other ones.
      tag.setAttribute("data-vue-router-controlled", "");

      return tag;
    })
    // Add the meta tags to the document head.
    .forEach((tag: any) => document.head.appendChild(tag));

  next();
});
router.afterEach((to, from) => {
  mevent("virtual_page_view", {
    virtualPagePath: to.path,
    virtualPageTitle: to.name,
    virtualPageReferer: from.path,
  });
});
export default router;

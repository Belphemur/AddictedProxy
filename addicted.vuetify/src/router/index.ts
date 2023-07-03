// Composables
import {createRouter, createWebHistory} from 'vue-router'
import {mevent} from "@/composables/matomo/event";

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/default/Default.vue'),
    children: [
      {
        path: '',
        name: 'Search',
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import(/* webpackChunkName: "home" */ '@/views/Home.vue'),
        meta: {
          order: 10,
          icon: "mdi-search-web",
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
          icon: "mdi-code-braces",
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
          import(/* webpackChunkName: "api" */ "@/views/ApiView.vue"),
      },
      {
        path: "/Privacy",
        name: "Privacy Policy",
        meta: {
          order: 30,
          icon: "mdi-shield-account",
          title: "Gestdown: Privacy Policy",
          metaTags: [
            {
              name: "description",
              content: "Privacy Policy of Gestdown",
            },
            {
              property: "og:description",
              content: "Privacy Policy of Gestdown",
            },
          ],
        },
        // route level code-splitting
        // this generates a separate chunk (api.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () =>
          import(/* webpackChunkName: "terms" */ "@/views/Privacy.vue"),
      },
      {
        path: 'shows/:showId/:showName',
        name: 'Show',
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import(/* webpackChunkName: "media" */ '@/views/MediaViewAsync.vue'),
        meta: {
          title: "Gestdown: :showName",
          metaTags: [
            {
              name: "description",
              content:
                "All the subtitles available for the :showName",
            },
            {
              property: "og:description",
              content:
                "Perfect place to find all the subtitles available for the :showName",
            },
          ],
        },
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory("/"),
  routes,
  scrollBehavior() {
    document.getElementById("app")?.scrollIntoView({behavior: "smooth"});
  },
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

  let title = ""

  // If a route with a title was found, set the document (page) title to that value.
  if (nearestWithTitle) {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    title = nearestWithTitle.meta.title;
  } else if (previousNearestWithMeta) {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    title = previousNearestWithMeta.meta.title;
  }
  Object.keys(to.params).forEach((key) => {
    // @ts-ignore
    title = title.replace(`:${key}`, to.params[key]);
  });
  document.title = title;

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
        let tagElem = tagDef[key]
        Object.keys(to.params).forEach((paramKey) => {
          tagElem = tagElem.replace(`:${paramKey}`, to.params[paramKey]);
        });
        tag.setAttribute(key, tagElem);
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

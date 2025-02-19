import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import SearchView from "../views/SearchView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: HomeView,
      meta: {
        title: "Homepage"
      }
    },
    {
      path: "/zoeken",
      name: "zoeken",
      component: SearchView,
      meta: {
        title: "Zoeken"
      }
    },
    {
      path: "/publicaties/:uuid",
      name: "publicatie",
      component: () => import("../views/PublicatieView.vue"),
      props: true,
      meta: {
        title: "Publicatie"
      }
    },
    {
      path: "/documenten/:uuid",
      name: "document",
      component: () => import("../views/DocumentView.vue"),
      props: true,
      meta: {
        title: "Document"
      }
    }
  ]
});

router.beforeEach(async (to, from) => {
  document.title = `${to.meta?.title || ""} | ${import.meta.env.VITE_APP_TITLE}`;
  // keep focus if only the query params are different
  if (to.path === from.path) {
    return;
  }
  document.body.setAttribute("tabindex", "-1");
  document.body.focus();
  document.body.removeAttribute("tabindex");
});

export default router;

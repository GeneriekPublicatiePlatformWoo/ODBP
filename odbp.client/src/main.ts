import "./assets/main.scss";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import { registerComponents } from "@/components/register";
import { loadThemeResources } from "./resources";

const app = createApp(App);

app.use(router);

registerComponents(app);

(async () => {
  // Load external theme resources before app mounts to prevent layout shifts
  await loadThemeResources(app);

  app.mount("#app");
})();

import "./assets/main.scss";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import { registerComponents } from "@/components/register";
import { loadThemeResources } from "./resources";
import { loadWaardelijsten } from "./stores/waardelijsten";

const app = createApp(App);

app.use(router);

registerComponents(app);

(async () => {
  // Load external theme resources before app mounts to prevent layout shifts
  await loadThemeResources(app);

  // Preload waardelijsten to be used in different app components
  await loadWaardelijsten();

  app.mount("#app");
})();

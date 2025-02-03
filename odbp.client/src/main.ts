import "./assets/main.scss";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import { registerComponents } from "@/components/register";
import { handleResources } from "./resources";

const app = createApp(App);

app.use(router);

registerComponents(app);

(async () => {
  await handleResources(app);

  app.mount("#app");
})();

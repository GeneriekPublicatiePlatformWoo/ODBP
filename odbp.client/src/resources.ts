import type { App } from "vue";

export type Resources = Partial<{
  website: string;
  logo: string;
  favicon: string;
  image: string;
  tokens: string;
  theme: string;
}>;

const getResources = async (): Promise<Resources> => {
  try {
    const response = await fetch("/api/environment/resources");
    return await response.json();
  } catch {
    return {};
  }
};

const setTheme = (theme?: string) => theme && document.querySelector("#app")?.classList.add(theme);

const setIcon = (href?: string) => {
  if (!href) return;

  const link = document.querySelector("link[rel~='icon']") as HTMLLinkElement;

  link.href = href;
  link.type = href.endsWith(".svg") ? "image/svg+xml" : "image/x-icon";
};

const validSources = (sources: (string | undefined)[]) =>
  sources.filter((url): url is string => typeof url === "string" && url.trim() !== "");

const addCssRefToMainLayer = (href: string) =>
  document.head.appendChild(
    Object.assign(document.createElement("style"), {
      textContent: `@import url("${href}") layer(main);`
    })
  );

const preloadResources = async (sources: (string | undefined)[]) => {
  const promises = validSources(sources).map((href) => {
    return new Promise<{ href: string }>((resolve, reject) => {
      const link = document.createElement("link");

      link.rel = "preload";
      link.as = href.endsWith(".css") ? "style" : "image";
      link.href = href;

      link.onload = () => {
        link.as === "style" && addCssRefToMainLayer(href);

        resolve({ href });
      };

      link.onerror = () => reject({ href });

      document.head.appendChild(link);
    });
  });

  const results = await Promise.allSettled(promises);

  const rejected = results.filter((result) => result.status === "rejected");

  if (rejected.length) throw new Error(rejected.map((r) => r.reason.href).join(", "));
};

export const handleResources = async (app: App): Promise<void> => {
  const resources = await getResources();

  try {
    await preloadResources([resources.tokens, resources.logo, resources.image]);

    setTheme(resources.theme);

    setIcon(resources.favicon);

		// Make references to resources available for app when all preloads are fulfilled
		app.provide("resources", resources);
  } catch (error) {
    console.error(`One or more external resources failed to load. ${error}`);
  }
};

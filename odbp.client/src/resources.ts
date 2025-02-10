import { inject, type App } from "vue";

export type Resources = Partial<{
  name: string;
  website: string;
  logo: string;
  favicon: string;
  image: string;
  tokens: string;
  theme: string;
}>;

export const injectResources = () => inject<Resources | null>("resources", null);

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

const loadResources = async (sources: (string | undefined)[]) => {
  const promises = validSources(sources).map((href) => {
    return new Promise<{ href: string }>((resolve, reject) => {
      const link = document.createElement("link");

      link.rel = href.endsWith(".css") ? "stylesheet" : "preload";
      link.rel === "preload" && (link.as = "image");
      link.href = href;
      link.crossOrigin = "anonymous";

      link.onload = () => resolve({ href });
      link.onerror = () => reject({ href });

      document.head.appendChild(link);
    });
  });

  const results = await Promise.allSettled(promises);

  const rejected = results.filter(
    (result): result is PromiseRejectedResult => result.status === "rejected"
  );

  if (rejected.length) throw new Error(rejected.map((r) => r.reason.href).join(", "));
};

export const loadThemeResources = async (app: App): Promise<void> => {
  // First fetch the references to external resources from the API
  const resources = await getResources();

  try {
    // Then load the external resources if provided: theme tokens, logo, and image
    // (this is done before mounting the app to prevent layout shifts)
    // Tokens will be loaded directly (as unlayered css, to be sure it takes precedence over the layered project css)
    // Images will be preloaded, waiting to be referenced from the app
    await loadResources([resources.tokens, resources.logo, resources.image]);

    // Replace the provided favicon link
    setIcon(resources.favicon);

    // Apply the associated theme class to the root element of the app
    setTheme(resources.theme);

    // Provide references to the loaded resources to the app
    app.provide("resources", resources);
  } catch (error) {
    // Log an error if any external resources fail to load
    console.error(`One or more external resources failed to load. ${error}`);
  }
};

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

const setIcon = (href?: string) => {
  if (!href) return;
  
  const link = document.querySelector("link[rel~='icon']") as HTMLLinkElement;

  link.href = href;
  link.type = href.endsWith(".svg") ? "image/svg+xml" : "image/x-icon";
};

const validSources = (sources: (string | undefined)[]) =>
  sources.filter((url): url is string => typeof url === "string" && url.trim() !== "");

const appendCss = async (sources: (string | undefined)[]) => {
  const promises = validSources(sources).map((href) => {
    return new Promise((resolve, reject) => {
      const link = document.createElement("link");

      link.rel = "stylesheet";
      link.href = href;
      link.onload = () => resolve(href);
      link.onerror = (error) => reject(error);

      document.head.appendChild(link);
    });
  });

  const results = await Promise.allSettled(promises);

  results.forEach((result) => console.log(result));
};

const preloadImages = async (sources: (string | undefined)[]) => {
  const promises = validSources(sources).map((src) => {
    return new Promise((resolve, reject) => {
      const img = new Image();

      img.src = src;
      img.onload = () => resolve(src);
      img.onerror = (error) => reject(error);
    });
  });

  const results = await Promise.allSettled(promises);

  results.forEach((result) => console.log(result));
};

export const handleResources = async (app: App): Promise<void> => {
  const resources = await getResources();

  setIcon(resources?.favicon);

  await appendCss([resources?.tokens]);

  await preloadImages([resources?.logo, resources?.image]);

  app.provide("resources", resources);
};

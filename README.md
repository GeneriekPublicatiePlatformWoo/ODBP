# Burgerportaal

## Omgevingsvariabelen

| Variabele                      | Uitleg                                                                                                                                                                                                                                                           |
| ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `ODRC_BASE_URL`                | De base url van de ODRC (Registratiecomponent) waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `https://odrc.mijn-gemeente.nl` </details>                                                                              |
| `ODRC_API_KEY`                 | De geheime sleutel voor de ODRC (Registratiecomponent) waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `VM2B!ccnebNe.M*gxH63*NXc8iTiAGhp`</details>                                                                    |
| `GEMEENTE_NAAM`                | De naam van de gemeente die wordt gebruikt binnen het burgerportaal. <details><summary>Meer informatie</summary> Bijvoorbeeld: Mijn</details>                                                                                                                    |
| `GEMEENTE_WEBSITE_URL`         | Het website-adres van de gemeente, om vanuit het burgerportaal naar de website van de gemeente te linken. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl`</details>                                                     |
| `GEMEENTE_LOGO_URL`            | Publiek URL waar het logo van de gemeente beschikbaar is. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/logo.svg`</details>                                                                                            |
| `GEMEENTE_FAVICON_URL`         | Publiek URL waar het favicon van de gemeente beschikbaar is. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/favicon.ico`</details>                                                                                      |
| `GEMEENTE_MAIN_IMAGE_URL`      | Publiek URL waar een hoge resolutie sfeerfoto van de gemeente beschikbaar is, die wordt opgenomen in alle pagina's. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/main_img.jpg` </details>                             |
| `GEMEENTE_DESIGN_TOKENS_URL`   | Publiek URL waar het css-bestand met NL Design System tokens beschikbaar is, om het burgerportaal te stylen in gemeentehuisstijl. <details><summary>Meer informatie </summary>Bijvoorbeeld: `https://unpkg.com/@gemeente/design-tokens/dist/index.css`</details> |
| `GEMEENTE_THEME_NAAM`          | De naam van de selector uit het css-bestand die wordt gebruikt om de NLDS-tokens te scopen. <details><summary>Meer informatie</summary> Bijvoorbeeld: `gemeente-theme` </details>                                                                                |
| `DOWNLOAD_TIMEOUT_MINUTES`     | Het aantal minuten dat het downloaden van bestanden maximaal mag duren. <br/> (default waarde is `10`)                                                                                                                                                           |
| `SITEMAP_CACHE_DURATION_HOURS` | Het aantal uur dat de sitemap in de cache bewaard wordt. <br/> (default waarde is `23`)                                                                                                                                                                          |

## NLDS – NL Design System

De interface van dit portaal is opgebouwd met componenten uit het **NL Design System (NLDS)**. Dit is een verzameling ontwerp- en ontwikkelrichtlijnen voor digitale overheidsdiensten in Nederland. Door gebruik te maken van NLDS-componenten blijft de gebruikerservaring consistent en toegankelijk, in lijn met de standaarden van de overheid.

🔗 Meer informatie: [Introductie NLDS](https://nldesignsystem.nl/handboek/introductie)

### Aanpasbaarheid voor gemeentes

Dankzij NLDS kunnen verschillende installaties van dit portaal eenvoudig worden aangepast aan de huisstijl van diverse gemeentes. Dit wordt mogelijk gemaakt door het gebruik van design tokens, die de stijlkenmerken zoals kleuren, typografie en componenten bepalen.

### Implementatie op basis van Utrecht Design System

Op dit moment is de implementatie gebaseerd op gemeentes alleen het Utrecht Design System, een specifieke variant van NLDS. Voor een correcte weergave en de beste resultaten moeten ten minste de Brand en Common tokens correct ingevuld zijn.

### Gebruikte CSS-componenten

| Component                         | Storybook                                                                                                             |
| --------------------------------- | --------------------------------------------------------------------------------------------------------------------- |
| **Document** (`utrecht-document`) | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-document--design-tokens) |
| **Surface** (`utrecht-surface`)   | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-surface--design-tokens)  |

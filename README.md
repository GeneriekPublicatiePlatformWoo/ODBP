# Burgerportaal

## Omgevingsvariabelen

| Variabele                              | Uitleg                                                                                                                                                                                                                                                           |
| -------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `ODRC_BASE_URL`                        | De base url van de ODRC (Registratiecomponent) waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `https://odrc.mijn-gemeente.nl` </details>                                                                              |
| `ODRC_API_KEY`                         | De geheime sleutel voor de ODRC (Registratiecomponent) waarmee gekoppeld moet worden. <details> <summary>Meer informatie </summary>Bijvoorbeeld: `VM2B!ccnebNe.M*gxH63*NXc8iTiAGhp`</details>                                                                    |
| `RESOURCES:GEMEENTE_NAAM`              | De naam van de gemeente die wordt gebruikt binnen het burgerportaal. <details><summary>Meer informatie</summary> Bijvoorbeeld: Mijn</details>                                                                                                                    |
| `RESOURCES:GEMEENTE_WEBSITE_URL`       | Het website-adres van de gemeente, om vanuit het burgerportaal naar de website van de gemeente te linken. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl`</details>                                                     |
| `RESOURCES:GEMEENTE_LOGO_URL`          | Publiek URL waar het logo van de gemeente beschikbaar is. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/logo.svg`</details>                                                                                            |
| `RESOURCES:GEMEENTE_FAVICON_URL`       | Publiek URL waar het favicon van de gemeente beschikbaar is. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/favicon.ico`</details>                                                                                      |
| `RESOURCES:GEMEENTE_MAIN_IMAGE_URL`    | Publiek URL waar een hoge resolutie sfeerfoto van de gemeente beschikbaar is, die wordt opgenomen in alle pagina's. <details><summary>Meer informatie</summary> Bijvoorbeeld: `https://www.mijn-gemeente.nl/main_img.jpg` </details>                             |
| `RESOURCES:GEMEENTE_DESIGN_TOKENS_URL` | Publiek URL waar het css-bestand met NL Design System tokens beschikbaar is, om het burgerportaal te stylen in gemeentehuisstijl. <details><summary>Meer informatie </summary>Bijvoorbeeld: `https://unpkg.com/@gemeente/design-tokens/dist/index.css`</details> |
| `RESOURCES:GEMEENTE_THEME_NAAM`        | De naam van de selector uit het css-bestand die wordt gebruikt om de NLDS-tokens te scopen. <details><summary>Meer informatie</summary> Bijvoorbeeld: `gemeente-theme` </details>                                                                                |
| `DOWNLOAD_TIMEOUT_MINUTES`             | Het aantal minuten dat het downloaden van bestanden maximaal mag duren. <br/> (default waarde is `10`)                                                                                                                                                           |
| `SITEMAP_CACHE_DURATION_HOURS`         | Het aantal uur dat de sitemap in de cache bewaard wordt. <br/> (default waarde is `23`)                                                                                                                                                                          |

## Cross-Origin Resource Sharing (CORS) en Cross-Origin-Embedder-Policy (COEP)

Deze applicatie maakt gebruik van Cross-Origin-Embedder-Policy (COEP: require-corp), maar de externe resources (afbeeldingen en stylesheets) worden geladen onder CORS (met cross-origin-attributen). Dat betekent dat die externe resources de juiste Access-Control-Allow-Origin-header moeten bevatten.

Alleen moet vanwege COEP voor het favicon ook de Cross-Origin-Resource-Policy-header gezet worden, omdat sommige browsers bij `<link rel=icon crossorigin>` het cross-origin-attribuut negeren en de resource geladen wordt in zogenaamde `no-cors` mode.

### Headers:

Voor alle externe resources moet `Access-Control-Allow-Origin: *` of bijvoorbeeld `Access-Control-Allow-Origin: *.mijn-gemeente.nl` ingesteld worden.

En voor het favicon naast de CORS-header ook CORP-header `Cross-Origin-Resource-Policy: cross-origin` instellen.

Als een resource niet correct is geconfigureerd, zal deze niet geladen worden door de browser.

## NLDS – NL Design System

De interface van het Burgerportaal is opgebouwd met componenten uit het **NL Design System (NLDS)**. Dit is een verzameling ontwerp- en ontwikkelrichtlijnen voor digitale overheidsdiensten in Nederland. Door gebruik te maken van NLDS-componenten blijft de gebruikerservaring consistent en toegankelijk, in lijn met de standaarden van de overheid.

🔗 Meer informatie: [Introductie NLDS](https://nldesignsystem.nl/handboek/introductie)

### Aanpasbaarheid voor gemeentes

Dankzij NLDS kunnen verschillende installaties van het Burgerportaal eenvoudig worden aangepast aan de huisstijl van diverse gemeentes. Dit wordt mogelijk gemaakt door het gebruik van design tokens, die de stijlkenmerken zoals kleuren, typografie en componenten bepalen.

### Implementatie op basis van Utrecht Design System

Op dit moment is de implementatie gebaseerd op alleen het Utrecht Design System, een specifieke variant van NLDS. Voor een correcte weergave en de beste resultaten moeten ten minste de Brand en Common tokens correct ingevuld zijn.

### Gebruikte CSS-componenten

| Component                                     | Storybook                                                                                                                   |
| :-------------------------------------------- | :-------------------------------------------------------------------------------------------------------------------------- |
| **Document** (`utrecht-document`)             | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-document--design-tokens)       |
| **Surface** (`utrecht-surface`)               | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-surface--design-tokens)        |
| **Page** (`utrecht-page`)                     | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page--design-tokens)           |
| **Page header** (`utrecht-page-header`)       | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page-header--design-tokens)    |
| **Page content** (`utrecht-page-content`)     | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page-content--design-tokens)   |
| **Page footer** (`utrecht-page-footer`)       | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-page-footer--design-tokens)    |
| **Navigation bar** (`utrecht-nav-bar`)        | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-nav-bar--design-tokens)        |
| **Link** (`utrecht-link`)                     | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-link--design-tokens)           |
| **Skip link** (`utrecht-skip-link`)           | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-skip-link--design-tokens)      |
| **Article** (`utrecht-article`)               | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-article--design-tokens)        |
| **Heading** (`utrecht-heading`)               | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-heading-1--design-tokens)      |
| **Paragraph** (`utrecht-paragraph`)           | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-paragraph--design-tokens)      |
| **Unordered list** (`utrecht-unordered-list`) | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-unordered-list--design-tokens) |
| **Button** (`utrecht-button`)                 | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-button--design-tokens)         |
| **Form field** (`utrecht-form-field`)         | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-form-field--design-tokens)     |
| **Form label** (`utrecht-form-label`)         | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-form-label--design-tokens)     |
| **Textbox** (`utrecht-textbox`)               | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-textbox--design-tokens)        |
| **Table** (`utrecht-table`)                   | [🔗 Design Tokens](https://nl-design-system.github.io/utrecht/storybook/?path=/story/css_css-table--design-tokens)          |

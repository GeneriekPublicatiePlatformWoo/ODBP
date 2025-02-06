import type { App } from "vue";

import {
  PageHeader as UtrechtPageHeader,
  PageFooter as UtrechtPageFooter,
  SkipLink as UtrechtSkipLink,
  Article as UtrechtArticle,
  Heading as UtrechtHeading,
  Paragraph as UtrechtParagraph,
  UnorderedList as UtrechtUnorderedList,
  UnorderedListItem as UtrechtUnorderedListItem,
  Button as UtrechtButton,
  FormField as UtrechtFormField,
  FormLabel as UtrechtFormLabel,
  Textbox as UtrechtTextbox
} from "@utrecht/component-library-vue";

const components = {
  UtrechtPageHeader,
  UtrechtPageFooter,
  UtrechtSkipLink,
  UtrechtArticle,
  UtrechtHeading,
  UtrechtParagraph,
  UtrechtUnorderedList,
  UtrechtUnorderedListItem,
  UtrechtButton,
  UtrechtFormField,
  UtrechtFormLabel,
  UtrechtTextbox
} as const;

export type OurComponents = typeof components;

export const registerComponents = (app: App): void =>
  Object.entries(components).forEach(([key, value]) => app.component(key, value));

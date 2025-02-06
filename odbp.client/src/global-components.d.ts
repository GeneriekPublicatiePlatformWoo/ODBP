import { OurComponents } from "./components/register";

declare module "@vue/runtime-core" {
  export interface GlobalComponents extends OurComponents {}
}

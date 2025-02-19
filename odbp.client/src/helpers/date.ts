export const formatDate = (date?: string) =>
  date && Intl.DateTimeFormat("nl-NL", { dateStyle: "long" }).format(Date.parse(date));

export const formatDate = (date?: string) =>
  date && Intl.DateTimeFormat("default", { dateStyle: "long" }).format(Date.parse(date));

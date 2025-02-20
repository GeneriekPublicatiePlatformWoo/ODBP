export const formatDate = (date?: string) =>
  date && Intl.DateTimeFormat("nl-NL", { dateStyle: "long" }).format(Date.parse(date));

const isoDateFormat = Intl.DateTimeFormat("sv-SE", {
  year: "numeric",
  day: "2-digit",
  month: "2-digit"
});

export const formatIsoDate = (date?: string | null | Date) =>
  !date ? null : isoDateFormat.format(new Date(date));

export const addToDate = (
  d: Date | string | null | undefined,
  addition: { year?: number; month?: number; day?: number }
) => {
  if (!d) return null;
  d = new Date(d);
  let year = d.getFullYear();
  let month = d.getMonth();
  let day = d.getDate();
  if (addition.year !== undefined) {
    year += addition.year;
  }
  if (addition.month !== undefined) {
    month += addition.month;
  }
  if (addition.day !== undefined) {
    day += addition.day;
  }
  return new Date(year, month, day);
};

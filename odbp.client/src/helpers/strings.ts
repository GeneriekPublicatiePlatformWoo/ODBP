export const truncate = (s: string, ch: number) => {
  if (s.length <= ch) return s;
  return s.substring(0, ch) + "...";
};

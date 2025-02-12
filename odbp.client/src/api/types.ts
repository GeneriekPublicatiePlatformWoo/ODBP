export type PagedResult<T> = Readonly<{
  count: number;
  next?: string;
  previous?: string;
  results: T[];
}>;

import type { UtrechtPaginationProps } from "@/components/UtrechtPagination.vue";
import type { RouteLocationRaw } from "vue-router";

type PaginationParams<T> = {
  pagination: {
    count: number;
    results: readonly T[];
    next: boolean;
    previous: boolean;
  };
  page: number;
  getRoute: (v: number) => RouteLocationRaw;
};

export function mapPaginatedResultsToUtrechtPagination<T>({
  page,
  getRoute,
  pagination: { count, results, next, previous }
}: PaginationParams<T>): UtrechtPaginationProps {
  const pageSize =
    next || !previous
      ? // if there is a next page, the current page length is the expected page size
        // if there is only one page, all we know is the length of that page
        results.length
      : // otherwise, we can calculate the page size
        (count - results.length) / (page - 1);

  const totalPages = Math.ceil(count / pageSize);

  const getLink = (i: number) => ({
    href: getRoute(i),
    title: `Resultaat ${(i - 1) * pageSize + 1} tot ${Math.min(count, i * pageSize)}`,
    current: i === page,
    number: i
  });

  const links = [];

  // we want to show a maximum of 7 pages, but the current page will not always be in the middle
  const max = 7;
  const maxDynamicCount = max - 2;

  let lower = page;
  let upper = lower;

  // thats why we want to calculate the minimum and maximum page here
  while (upper - lower < maxDynamicCount - 1 && (lower > 2 || totalPages - 1 > upper)) {
    lower = Math.max(2, lower - 1);
    upper = Math.min(totalPages - 1, upper + 1);
  }

  links.push(getLink(1));

  for (let index = 0; index <= upper - lower; index++) {
    const number = index + lower;
    if (number > 1 && number < totalPages) {
      links.push(getLink(number));
    }
  }

  if (totalPages > 1) {
    links.push(getLink(totalPages));
  }

  return {
    links,
    currentIndex: page,
    prev: page > 1 ? getLink(page - 1) : undefined,
    next: page < totalPages ? getLink(page + 1) : undefined
  };
}

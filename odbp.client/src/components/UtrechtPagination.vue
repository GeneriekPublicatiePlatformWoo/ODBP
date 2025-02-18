<template>
  <nav class="utrecht-pagination">
    <span class="utrecht-pagination__before" v-if="utrechtProps.prev">
      <router-link
        :to="utrechtProps.prev.href"
        class="utrecht-pagination__relative-link utrecht-pagination__relative-link--prev"
        rel="prev"
        :replace="true"
      >
        Vorige
      </router-link>
    </span>
    <span role="group" class="utrecht-pagination__pages">
      <router-link
        v-for="link in utrechtProps.links"
        :key="link.number"
        :class="[
          'utrecht-pagination__page-link',
          link.current && 'utrecht-pagination__page-link--current'
        ]"
        :to="link.href"
        :replace="true"
        :aria-current="link.current ? 'true' : undefined"
        :title="link.title"
      >
        {{ link.number }}
      </router-link>
    </span>
    <span class="utrecht-pagination__before" v-if="utrechtProps.next">
      <router-link
        :to="utrechtProps.next.href"
        :replace="true"
        class="utrecht-pagination__relative-link utrecht-pagination__relative-link--next"
        rel="next"
      >
        Volgende
      </router-link>
    </span>
  </nav>
</template>

<script setup lang="ts">
import { computed } from "vue";
import type { RouteLocationRaw } from "vue-router";

const props = defineProps<{
  pagination: {
    count: number;
    results: unknown[];
    next: boolean;
    previous: boolean;
  };
  page: number;
  getRoute: (v: number) => RouteLocationRaw;
}>();

const utrechtProps = computed(() => {
  const {
    page,
    getRoute,
    pagination: { count, results, next, previous }
  } = props;

  const pageSize =
    next || !previous
      ? // if there is a next page, the current page length is the expected page size
        // if there is only one page, all we know is the length of that page
        results.length
      : // otherwise, we can calculate the page size
        (count - results.length) / (page - 1);

  const totalPages = Math.ceil(props.pagination.count / pageSize);

  const getLink = (i: number) => ({
    href: getRoute(i),
    title: `Resultaat ${(i - 1) * pageSize + 1} tot ${Math.min(count, i * pageSize)}`,
    current: i === page,
    number: i
  });

  // we want to show a maximum of 4 pages, but the current page will not always be in the middle
  const max = 4;

  let lower = page;
  let upper = lower;

  // thats why we want to calculate the minimum and maximum page here
  while (upper - lower < max && (lower > 1 || totalPages > upper)) {
    lower = Math.max(1, lower - 1);
    upper = Math.min(totalPages, upper + 1);
  }

  const links = [];

  if (lower > 1) {
    links.push(getLink(1));
  }

  for (let index = 0; index <= upper - lower; index++) {
    const number = index + lower;
    links.push(getLink(number));
  }

  if (totalPages > upper) {
    links.push(getLink(totalPages));
  }

  return {
    links,
    currentIndex: page,
    prev: previous ? getLink(page - 1) : undefined,
    next: next ? getLink(page + 1) : undefined
  };
});
</script>

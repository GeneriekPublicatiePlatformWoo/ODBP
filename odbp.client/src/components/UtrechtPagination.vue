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
    next?: string;
    previous?: string;
  };
  getRoute: (v: number) => RouteLocationRaw;
}>();

const page = defineModel<number>("page", { required: true });

const utrechtProps = computed(() => {
  const pageSize =
    props.pagination.next || !props.pagination.previous
      ? props.pagination.results.length
      : (props.pagination.count - props.pagination.results.length) / (page.value - 1);

  const totalPages = Math.ceil(props.pagination.count / pageSize);

  const getLink = (i: number) => ({
    href: props.getRoute(i),
    title: `Resultaat ${i * pageSize + 1} tot ${Math.min(props.pagination.count, (i + 1) * pageSize)}`,
    current: i === page.value,
    number: i
  });

  const max = 4;

  let lower = page.value;
  let upper = page.value;

  while (upper - lower < max && (lower > 1 || totalPages > upper)) {
    lower = Math.max(1, lower - 1);
    upper = Math.min(totalPages, upper + 1);
  }

  const links = [getLink(1)];

  for (let index = 0; index <= upper - lower; index++) {
    const number = index + lower;
    if (number !== 1 && number !== totalPages) {
      links.push(getLink(number));
    }
  }

  links.push(getLink(totalPages));

  const prev = props.pagination.previous ? getLink(page.value - 1) : undefined;
  const next = props.pagination.next ? getLink(page.value + 1) : undefined;

  return {
    currentIndex: page.value,
    links,
    prev,
    next
  };
});
</script>

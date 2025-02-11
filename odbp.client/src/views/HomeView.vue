<template>
  <utrecht-article>
    <utrecht-heading :level="1">Het Woo Burgerportaal</utrecht-heading>

    <utrecht-paragraph>
      Zoek hier naar documenten die de gemeente openbaar heeft gemaakt. Deze documenten hebben te
      maken met de ideeën, plannen, regels en afspraken. Zoek op een trefwoord, bijvoorbeeld
      “zonnepanelen” of “windmolens”.
    </utrecht-paragraph>

    <utrecht-heading :level="2">Wat kunt u hier vinden?</utrecht-heading>

    <utrecht-paragraph>
      De Wet Open Overheid (Woo) regelt landelijk welke soorten informatie door overheden actief
      openbaar gemaakt moeten worden. De gemeente werkt de komende jaren aan de aanpassingen die
      nodig zijn om dit te doen. Er zijn 11 categorieën van informatie aangewezen die gemeenten
      actief openbaar moeten maken. We noemen dit Woo-categorieën. Naast Woo-categorieën heeft de
      gemeente ook andere onderwerpen waarover zij informatie actief openbaar maakt, die noemen we
      overige onderwerpen.
    </utrecht-paragraph>

    <utrecht-heading :level="2">Heeft u nog niet alles gevonden?</utrecht-heading>

    <utrecht-paragraph>
      Misschien vindt u de gezochte informatie op één van de volgende websites:
    </utrecht-paragraph>

    <utrecht-unordered-list>
      <utrecht-unordered-list-item>
        <router-link
          :to="{
            name: 'publicatie',
            params: { uuid: 'a3e0aedb-952c-4721-af87-45422f33ff35' }
          }"
          class="utrecht-link utrecht-link--html-a"
          >Nieuwste publicatie</router-link
        >
      </utrecht-unordered-list-item>

      <utrecht-unordered-list-item>
        <router-link
          :to="{
            name: 'publicatie',
            params: { uuid: 'a425f67b-2bd3-40bf-bb4b-a54a64f5d0ef' }
          }"
          class="utrecht-link utrecht-link--html-a"
          >Nieuwe publicatie met 7 bestanden</router-link
        >
      </utrecht-unordered-list-item>
    </utrecht-unordered-list>
  </utrecht-article>
</template>

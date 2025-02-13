<template>
  <utrecht-page-footer>
    <div class="utrecht-page-footer__content">
      <div class="utrecht-page-footer__navigation">
        <ul role="list" class="utrecht-link-list utrecht-link-list--html-ul">
          <li class="utrecht-link-list__item">
            <utrecht-link external :href="`/toegankelijkheid`" class="utrecht-link-list__link">
              <gpp-woo-icon icon="chevron-right" />

              <span class="utrecht-link-list__link-text">Toegankelijkheid</span>

              <span class="visually-hidden">(externe link)</span>

              <gpp-woo-icon icon="external" />
            </utrecht-link>
          </li>

          <li class="utrecht-link-list__item">
            <utrecht-link external :href="`/privacy`" class="utrecht-link-list__link">
              <gpp-woo-icon icon="chevron-right" />

              <span class="utrecht-link-list__link-text">Privacy</span>

              <span class="visually-hidden">(externe link)</span>
              
              <gpp-woo-icon icon="external" />
            </utrecht-link>
          </li>

          <li class="utrecht-link-list__item">
            <utrecht-link external :href="`/contact`" class="utrecht-link-list__link">
              <gpp-woo-icon icon="chevron-right" />

              <span class="utrecht-link-list__link-text">Contact</span>

              <span class="visually-hidden">(externe link)</span>
              
              <gpp-woo-icon icon="external" />
            </utrecht-link>
          </li>
        </ul>
      </div>
    </div>
  </utrecht-page-footer>
</template>

<script setup lang="ts">
import GppWooIcon from "@/components/GppWooIcon.vue";
</script>

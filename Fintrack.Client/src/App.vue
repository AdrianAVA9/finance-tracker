<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';

// The route meta will define which layout to use
const route = useRoute();
const layout = computed(() => {
  return route.meta.layout || 'div';
});
</script>

<template>
  <component :is="layout">
    <router-view />
  </component>
</template>

<style src="@/assets/main.scss"></style>

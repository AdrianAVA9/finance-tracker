<script setup lang="ts">
defineProps<{
  logoSrc: string;
  title: string;
  subtitle: string;
  logoClass?: string;
}>();
</script>

<template>
  <div class="flex flex-col items-center gap-4">
    <div :class="logoClass ?? 'h-16 w-16 rounded-2xl overflow-hidden shadow-lg shadow-[#1e6a7b]/10'">
      <img :src="logoSrc" alt="CeroBase Logo" class="h-full w-full object-cover" />
    </div>
    <div class="text-center">
      <h1 class="text-2xl font-bold tracking-tight text-slate-100">{{ title }}</h1>
      <p class="mt-2 text-sm text-slate-400">{{ subtitle }}</p>
    </div>
  </div>
</template>

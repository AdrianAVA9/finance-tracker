<script setup lang="ts">
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

const title = computed(() =>
  typeof route.meta.title === 'string' ? route.meta.title : 'Detalle'
);

function goBack() {
  if (typeof window !== 'undefined' && window.history.length > 1) {
    router.back();
  } else {
    router.push('/app/dashboard');
  }
}
</script>

<template>
  <div
    class="bg-background text-on-background font-body min-h-screen relative overflow-x-hidden selection:bg-primary-container selection:text-on-primary-container pb-safe"
  >
    <header
      class="fixed top-0 w-full z-50 bg-[#111317] flex items-center gap-3 px-4 h-16 border-b border-white/5 transition-all duration-200 ease-in-out"
    >
      <button
        type="button"
        class="w-10 h-10 shrink-0 flex items-center justify-center rounded-full hover:bg-[#2A2F37] transition-colors -ml-1"
        aria-label="Volver"
        @click="goBack"
      >
        <span class="material-symbols-outlined text-[#F3F4F6]">arrow_back</span>
      </button>
      <h1
        class="font-headline font-bold tracking-tighter text-[#F3F4F6] text-lg min-w-0 flex-1 truncate text-center pr-9"
      >
        {{ title }}
      </h1>
    </header>

    <div class="h-full relative w-full pt-20 px-4 max-w-2xl mx-auto">
      <router-view />
    </div>
  </div>
</template>


<script setup lang="ts">
import { useNetworkStatus } from '@/shared/composables/useNetworkStatus';

const { isOnline } = useNetworkStatus();
</script>

<template>
  <Transition
    enter-active-class="transition duration-300 ease-out"
    enter-from-class="transform -translate-y-full opacity-0"
    enter-to-class="transform translate-y-0 opacity-100"
    leave-active-class="transition duration-200 ease-in"
    leave-from-class="transform translate-y-0 opacity-100"
    leave-to-class="transform -translate-y-full opacity-0"
  >
    <div 
      v-if="!isOnline"
      class="sticky top-20 z-40 w-full bg-error-container/10 backdrop-blur-md border-b border-error/20 py-2.5 px-6 flex items-center justify-center gap-3"
    >
      <div class="flex items-center gap-2 px-3 py-1 rounded-full bg-error/10 border border-error/20">
        <span class="material-symbols-outlined text-[16px] text-error animate-pulse" style="font-variation-settings: 'FILL' 1;">cloud_off</span>
        <span class="text-[10px] font-label font-bold tracking-[0.1em] uppercase text-error">Sin conexión</span>
      </div>
      <p class="text-[11px] text-slate-400 font-medium">Sincronización pausada temporalmente</p>
    </div>
  </Transition>
</template>

<style scoped>
.font-label {
  font-family: 'Inter', sans-serif;
}
</style>

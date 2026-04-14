<script setup lang="ts">
defineProps<{
  isLoading?: boolean
  message?: string
}>()
</script>

<template>
  <div v-if="isLoading" class="flex-1 flex flex-col items-center justify-center min-h-[50vh] gap-4">
    <div class="w-10 h-1 bg-surface-container-highest rounded-full overflow-hidden relative">
      <div class="absolute inset-0 bg-primary-container w-1/3 animate-sk-loading"></div>
    </div>
    <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[.2em]">
      {{ message || 'Cargando Datos' }}
    </p>
  </div>
</template>

<style scoped>
@keyframes sk-loading {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(300%); }
}

.animate-sk-loading {
  animation: sk-loading 1.5s infinite;
}
</style>

<script setup lang="ts">
defineProps<{
  message?: string
}>()
</script>

<template>
  <div
    class="fixed inset-0 z-[9999] flex flex-col items-center justify-center bg-background text-on-background"
  >
    <div class="flex flex-col items-center gap-6">
      <div
        class="w-20 h-20 rounded-3xl bg-primary/10 flex items-center justify-center border border-primary/20 animate-pulse"
      >
        <span class="material-symbols-outlined text-primary text-5xl">wallet</span>
      </div>
      <div class="flex flex-col items-center gap-1">
        <h1 class="text-2xl font-bold tracking-tight font-headline">CeroBase</h1>
        <p class="text-on-surface-variant text-sm animate-pulse">
          {{ message ?? 'Sincronizando tus finanzas...' }}
        </p>
      </div>
      <div class="flex gap-1.5 mt-2">
        <div class="w-2 h-2 rounded-full bg-primary animate-bounce [animation-delay:-0.3s]" />
        <div class="w-2 h-2 rounded-full bg-primary animate-bounce [animation-delay:-0.15s]" />
        <div class="w-2 h-2 rounded-full bg-primary animate-bounce" />
      </div>
    </div>
  </div>
</template>

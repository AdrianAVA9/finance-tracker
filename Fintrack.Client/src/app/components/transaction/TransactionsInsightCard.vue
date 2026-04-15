<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
</script>

<template>
  <SurfaceCard class="editorial-shadow relative overflow-hidden rounded-2xl !border-[#05E699]/10 !bg-gradient-to-br !from-surface-container-low !to-surface-dim !p-8">
    <div class="relative z-10 space-y-3">
      <div class="flex items-center gap-3">
        <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-[#05E699]/10 text-[#05E699]">
          <span class="material-symbols-outlined text-lg">insights</span>
        </div>
        <p class="text-[10px] font-black uppercase tracking-[0.2em] text-[#05E699]">CEROBASE Insights</p>
      </div>
      <h4 class="font-headline text-lg font-black leading-tight text-on-surface">
        Has optimizado tus gastos un 12% este mes.
      </h4>
      <p class="max-w-[240px] text-xs leading-relaxed text-on-surface-variant">
        Tu estrategia de gasto en categorias esenciales ha mejorado la liquidez de tu portafolio.
      </p>
    </div>
    <div class="absolute -bottom-8 -right-8 h-40 w-40 rounded-full bg-[#05E699]/5 blur-3xl"></div>
    <span class="material-symbols-outlined absolute right-8 top-1/2 -translate-y-1/2 animate-pulse text-5xl text-[#05E699]/10">
      trending_up
    </span>
  </SurfaceCard>
</template>

<style scoped>
.editorial-shadow {
  box-shadow: 0 40px 60px -20px rgba(5, 230, 153, 0.04);
}
</style>

<script setup lang="ts">
defineProps<{
  logoSrc: string;
}>();
</script>

<template>
  <div class="relative hidden lg:flex lg:w-[60%] flex-col items-start justify-center p-16 overflow-hidden">
    <div class="absolute inset-0 z-0">
      <div class="absolute inset-0 bg-[#0F0F1A]/60 z-10" />
      <img
        src="https://lh3.googleusercontent.com/aida-public/AB6AXuB7ReZf5Sq5Fzs7O6OE-yOiB0wEaFhv3jQ8c7OVoLoeGcJb0ip8t_cLZoEIb_7yCmru6C7wZVjlV9DrKyGKvJZeBL56vO8kZmMWeYaO4wj6sFSoppf1tusQ7g9OnCdXK049-xVdZ26KXnETmwex4vcbEPeKnGyGWW5SSPKC8aSMEitVX8BfaMZTMJQMMd19i6fSehKcWKQYafZeSIUvB3208cS8YjDVDjt7gyP1aAUwFs7EkI6rnhC87mC8qk4y41x9MhvIbVM3bz3k"
        alt="Abstract digital network security visualization"
        class="h-full w-full object-cover"
      />
    </div>

    <div class="relative z-20 max-w-2xl">
      <div class="mb-12 flex items-center gap-3">
        <img :src="logoSrc" alt="CeroBase Logo" class="w-12 h-12 rounded-xl" />
        <span class="text-2xl font-bold tracking-tight text-white">CeroBase</span>
      </div>

      <h1 class="mb-6 text-5xl font-extrabold leading-[1.1] tracking-tight text-white lg:text-6xl">
        Comienza tu viaje financiero con precision.
      </h1>

      <p class="text-xl font-medium leading-relaxed text-slate-300">
        Crea una cuenta para proteger y hacer crecer tu patrimonio con seguridad avanzada. Nuestra tecnologia
        monitorea tus activos 24/7.
      </p>

      <div class="mt-12 flex gap-8">
        <div class="flex flex-col">
          <span class="text-3xl font-bold text-white">99.9%</span>
          <span class="text-sm uppercase tracking-widest text-[#85B2B2]">Deteccion de Amenazas</span>
        </div>
        <div class="flex flex-col">
          <span class="text-3xl font-bold text-white">256-bit</span>
          <span class="text-sm uppercase tracking-widest text-[#85B2B2]">Encriptacion</span>
        </div>
      </div>
    </div>

    <div class="absolute bottom-8 left-16 z-20 flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-[#AAAAAA]/50">
      <span>Seguridad de Nivel Empresarial</span>
      <span class="h-px w-12 bg-[#AAAAAA]/30" />
      <span>Impulsado por Inteligencia Neuronal</span>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  logoSrc: string;
}>();
</script>

<template>
  <div class="hidden lg:flex lg:w-[60%] relative h-full bg-[#0F0F1A] items-center justify-center overflow-hidden">
    <div
      class="absolute inset-0 w-full h-full bg-cover bg-center opacity-80"
      style="background-image: url('https://lh3.googleusercontent.com/aida-public/AB6AXuDERtLs02LEU4puPj5Yr231Z1mqpASVb3qTfzJH8hzYPvNyopc4uPXzhAH_iRN3wXarrhiSW1VATYYB6ur5HYskypAR8vtln6Phv_e8LOLzpqY7FnJ3Gn62anMJ3Nb6mxzeA2Ij5GC5XbfY8xflxvx9kW0FNsr-5JBXTJws5Tu5viMP20Sfpg2xKcsKXU8FPLfHiCPZTbUgMHEcH7ZaTTS-eiCkVlagJkJyrzLmhWsPU9dmHeSE-OHbY9mnjhTC71BSrvFm5gbyqAnb');"
    />
    <div class="absolute inset-0 bg-[#191971]/40 mix-blend-multiply" />
    <div class="absolute inset-0 bg-gradient-to-t from-[#0e0e1b] via-[#0e0e1b]/40 to-transparent" />
    <div class="absolute inset-0 bg-gradient-to-r from-transparent via-transparent to-[#0F0F1A]" />

    <div class="relative z-10 p-12 max-w-2xl text-left self-end mb-20">
      <div class="flex items-center gap-3 mb-6">
        <img :src="logoSrc" alt="CeroBase Logo" class="w-12 h-12 rounded-xl" />
        <p class="text-2xl font-bold tracking-wide">CeroBase</p>
      </div>
      <h2 class="text-4xl md:text-5xl font-bold leading-tight mb-4 text-white">
        Claridad financiera,<br />impulsada por inteligencia.
      </h2>
      <p class="text-lg text-[#9898c3] max-w-lg">
        Experimenta la próxima generación de gestión presupuestaria donde la precisión se une a la seguridad.
      </p>
    </div>
  </div>
</template>

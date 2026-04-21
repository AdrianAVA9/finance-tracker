<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';
</script>

<template>
  <SurfaceCard>
    <div class="flex flex-col items-center gap-4 py-2">
      <div class="rounded-full bg-emerald-500/20 p-4">
        <span class="material-symbols-outlined text-4xl text-emerald-400">check_circle</span>
      </div>
      <h2 class="text-xl font-bold text-white">Contraseña Actualizada</h2>
      <p class="text-center text-sm text-slate-400">
        Tu contraseña ha sido cambiada con éxito. Redirigiendo al inicio de sesión...
      </p>
    </div>
  </SurfaceCard>
</template>

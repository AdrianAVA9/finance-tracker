<script setup lang="ts">
import AppButton from '@/app/components/common/AppButton.vue';
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';

defineProps<{
  email: string;
}>();
</script>

<template>
  <SurfaceCard>
    <div class="flex flex-col items-center gap-4 py-2">
      <div class="rounded-full bg-emerald-500/20 p-4">
        <span class="material-symbols-outlined text-4xl text-emerald-400">mark_email_read</span>
      </div>
      <h2 class="text-xl font-bold text-slate-100">Revisa tu bandeja de entrada</h2>
      <p class="text-center text-sm text-slate-400">
        Si existe una cuenta con <span class="font-medium text-white">{{ email }}</span>, se ha enviado un enlace de recuperación.
      </p>
      <router-link to="/auth/login" class="mt-2 w-full">
        <AppButton variant="secondary" icon="arrow_back">
          Volver al Inicio de Sesión
        </AppButton>
      </router-link>
    </div>
  </SurfaceCard>
</template>

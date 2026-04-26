<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';

defineProps<{
  displayName: string;
  email: string;
}>();

const emit = defineEmits<{
  personalInfo: [];
  security: [];
  linkedBanks: [];
}>();
</script>

<template>
  <section class="space-y-4">
    <SurfaceCard class="!p-6">
      <div class="flex items-center gap-4">
        <div
          class="w-16 h-16 rounded-full border-2 border-primary-container/20 bg-surface-container-highest flex items-center justify-center overflow-hidden shrink-0"
        >
          <span class="material-symbols-outlined text-on-surface-variant text-3xl">person</span>
        </div>
        <div class="min-w-0">
          <h2 class="font-headline font-bold text-xl text-on-surface truncate">{{ displayName }}</h2>
          <p class="font-body text-on-surface-variant text-sm truncate">{{ email }}</p>
        </div>
      </div>
    </SurfaceCard>

    <div class="space-y-1">
      <button
        type="button"
        class="w-full flex items-center justify-between p-4 bg-surface-container hover:bg-surface-container-high transition-colors rounded-lg text-left group"
        @click="emit('personalInfo')"
      >
        <div class="flex items-center gap-4 min-w-0">
          <span
            class="material-symbols-outlined text-on-surface-variant group-hover:text-primary-container shrink-0"
            >person</span
          >
          <span class="font-body font-medium text-on-surface">Información personal</span>
        </div>
        <span class="material-symbols-outlined text-on-surface-variant shrink-0">chevron_right</span>
      </button>
      <button
        type="button"
        class="w-full flex items-center justify-between p-4 bg-surface-container hover:bg-surface-container-high transition-colors rounded-lg text-left group"
        @click="emit('security')"
      >
        <div class="flex items-center gap-4 min-w-0">
          <span
            class="material-symbols-outlined text-on-surface-variant group-hover:text-primary-container shrink-0"
            >fingerprint</span
          >
          <span class="font-body font-medium text-on-surface">Seguridad y biometría</span>
        </div>
        <span class="material-symbols-outlined text-on-surface-variant shrink-0">chevron_right</span>
      </button>
      <button
        type="button"
        class="w-full flex items-center justify-between p-4 bg-surface-container hover:bg-surface-container-high transition-colors rounded-lg text-left group"
        @click="emit('linkedBanks')"
      >
        <div class="flex items-center gap-4 min-w-0">
          <span
            class="material-symbols-outlined text-on-surface-variant group-hover:text-primary-container shrink-0"
            >account_balance</span
          >
          <span class="font-body font-medium text-on-surface">Cuentas bancarias vinculadas</span>
        </div>
        <span class="material-symbols-outlined text-on-surface-variant shrink-0">chevron_right</span>
      </button>
    </div>
  </section>
</template>

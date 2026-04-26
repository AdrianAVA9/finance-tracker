<script setup lang="ts">
import { ref } from 'vue';
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';

const notificationsOn = ref(true);

const emit = defineEmits<{
  currency: [];
  language: [];
}>();

function toggleNotifications() {
  notificationsOn.value = !notificationsOn.value;
}
</script>

<template>
  <section class="space-y-4">
    <h3
      class="font-headline text-xs font-black uppercase tracking-[0.2em] text-on-surface-variant px-1"
    >
      Preferencias
    </h3>
    <SurfaceCard class="!p-0 overflow-hidden">
      <div class="rounded-xl overflow-hidden">
        <button
          type="button"
          class="w-full flex items-center justify-between p-5 hover:bg-surface-container transition-colors text-left"
          @click="emit('currency')"
        >
          <div class="flex items-center gap-4 min-w-0">
            <span class="material-symbols-outlined text-on-surface-variant shrink-0"
              >currency_exchange</span
            >
            <span class="font-body font-medium text-on-surface">Moneda</span>
          </div>
          <span class="font-headline font-bold text-primary-container shrink-0">₡ (CRC)</span>
        </button>
        <div class="h-px bg-white/5 mx-5" />
        <button
          type="button"
          class="w-full flex items-center justify-between p-5 hover:bg-surface-container transition-colors text-left"
          @click="emit('language')"
        >
          <div class="flex items-center gap-4 min-w-0">
            <span class="material-symbols-outlined text-on-surface-variant shrink-0">language</span>
            <span class="font-body font-medium text-on-surface">Idioma</span>
          </div>
          <span class="font-headline font-bold text-on-surface-variant shrink-0">Español</span>
        </button>
        <div class="h-px bg-white/5 mx-5" />
        <div
          class="flex items-center justify-between p-5 hover:bg-surface-container transition-colors"
        >
          <div class="flex items-center gap-4 min-w-0">
            <span class="material-symbols-outlined text-on-surface-variant shrink-0"
              >notifications</span
            >
            <span class="font-body font-medium text-on-surface">Notificaciones</span>
          </div>
          <button
            type="button"
            class="w-10 h-5 bg-primary-container rounded-full relative shrink-0 transition-colors"
            :class="!notificationsOn && 'bg-surface-variant'"
            role="switch"
            :aria-checked="notificationsOn"
            @click="toggleNotifications"
          >
            <span
              class="absolute top-1 w-3 h-3 bg-on-primary rounded-full transition-all"
              :class="notificationsOn ? 'right-1' : 'left-1'"
            />
          </button>
        </div>
      </div>
    </SurfaceCard>
  </section>
</template>

<script setup lang="ts">
import { useRegisterSW } from 'virtual:pwa-register/vue'

const {
  offlineReady,
  needRefresh,
  updateServiceWorker,
} = useRegisterSW()

const close = () => {
  offlineReady.value = false
  needRefresh.value = false
}
</script>

<template>
  <div
    v-if="offlineReady || needRefresh"
    class="fixed right-0 bottom-0 m-6 p-4 border border-surface-container-highest rounded-2xl bg-surface-container-high/90 backdrop-blur-xl shadow-2xl z-[100] flex flex-col md:flex-row items-center gap-4 animate-in fade-in slide-in-from-bottom-4 duration-500"
    role="alert"
  >
    <div class="flex items-center gap-3">
      <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center border border-primary/20">
        <span class="material-symbols-outlined text-primary text-2xl">
          {{ offlineReady ? 'notifications_active' : 'system_update' }}
        </span>
      </div>
      <div class="flex flex-col">
        <span class="text-sm font-semibold text-on-surface">
          {{ offlineReady ? 'App lista para usar sin conexión' : 'Nueva versión disponible' }}
        </span>
        <p v-if="needRefresh" class="text-xs text-on-surface-variant">
          Actualiza para disfrutar de las últimas mejoras de CeroBase.
        </p>
      </div>
    </div>
    <div class="flex items-center gap-2 ml-auto w-full md:w-auto">
      <button
        v-if="needRefresh"
        @click="updateServiceWorker()"
        class="flex-1 md:flex-none px-4 py-2 bg-primary text-surface-dim font-bold text-sm rounded-xl hover:bg-primary/90 transition-colors shadow-lg shadow-primary/20"
      >
        Actualizar
      </button>
      <button
        @click="close"
        class="flex-1 md:flex-none px-4 py-2 bg-surface-container-highest/50 text-on-surface-variant hover:text-on-surface font-semibold text-sm rounded-xl transition-colors border border-on-surface/5"
      >
        {{ needRefresh ? 'Más tarde' : 'Cerrar' }}
      </button>
    </div>
  </div>
</template>

<style scoped>
@keyframes fade-in {
  from { opacity: 0; transform: translateY(1rem); }
  to { opacity: 1; transform: translateY(0); }
}
.animate-in {
  animation: fade-in 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}
</style>

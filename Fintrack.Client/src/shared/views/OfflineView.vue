<script setup lang="ts">
/**
 * OfflineView
 * Full-page component shown when user navigates while offline.
 * Implementation based on "Precision Luminescence" design.
 * Buttons are omitted as per user request.
 */
</script>

<template>
  <div class="bg-[#111317] text-white min-h-screen flex flex-col items-center justify-center p-6 overflow-hidden relative font-body">
    <!-- Header/Brand -->
    <header class="absolute top-0 w-full flex justify-between items-center px-6 py-8">
      <div class="text-lg font-bold tracking-tighter text-[#05E699] font-headline">Cero Fintech</div>
    </header>

    <main class="flex-1 flex flex-col items-center justify-center w-full max-w-md px-8 text-center z-10">
      <!-- Hero Visual Section: Precision Luminescence -->
      <div class="relative w-full aspect-square flex items-center justify-center mb-12 max-h-[300px]">
        <!-- Animated-like background glows -->
        <div class="absolute inset-0 bg-[#05E699]/5 rounded-full blur-[80px]"></div>
        <div class="absolute w-48 h-48 bg-[#05E699]/10 rounded-full blur-[40px]"></div>
        
        <!-- Icon Composition -->
        <div class="relative z-10 flex flex-col items-center">
          <div class="mb-4 bg-[#1a1c20] p-8 rounded-full border border-white/5 shadow-2xl">
            <span class="material-symbols-outlined text-[80px] text-[#77ffbb]" style="font-variation-settings: 'FILL' 0;">cloud_off</span>
          </div>
          
          <!-- Status Indicators -->
          <div class="flex gap-3 mt-6">
            <div class="flex items-center gap-2 px-3 py-1.5 rounded-full bg-[#282a2e] border border-white/10">
              <span class="material-symbols-outlined text-[14px] text-[#77ffbb]" style="font-variation-settings: 'FILL' 1;">verified_user</span>
              <span class="text-[10px] font-label font-medium tracking-wide uppercase text-slate-300">Resguardo Seguro</span>
            </div>
            <div class="flex items-center gap-2 px-3 py-1.5 rounded-full bg-[#282a2e] border border-white/10">
              <span class="material-symbols-outlined text-[14px] text-[#77ffbb]" style="font-variation-settings: 'FILL' 1;">database</span>
              <span class="text-[10px] font-label font-medium tracking-wide uppercase text-slate-300">Caché Local Activa</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Editorial Content Area -->
      <div class="space-y-6">
        <h1 class="font-headline font-extrabold text-4xl tracking-tight text-white leading-tight">
          Sincronización Interrumpida
        </h1>
        <p class="font-body text-md text-slate-400 leading-relaxed max-w-[280px] mx-auto">
          Cero está trabajando en modo sin conexión. Tus datos están seguros y se sincronizarán en cuanto vuelvas a estar en línea.
        </p>
      </div>
    </main>

    <!-- Contextual Information (Bottom Sheet Fragment) -->
    <footer class="w-full max-w-md pb-12">
      <div class="bg-[#1a1c20]/50 backdrop-blur-md rounded-2xl p-6 border border-white/5 relative overflow-hidden">
        <div class="absolute top-0 right-0 p-4 opacity-5">
          <span class="material-symbols-outlined text-4xl text-white">security</span>
        </div>
        <div class="flex items-start gap-4">
          <div class="mt-1">
            <span class="material-symbols-outlined text-[#77ffbb]">info</span>
          </div>
          <div class="text-left">
            <h3 class="font-headline font-bold text-sm text-white mb-1">Protección de Grado Bancario</h3>
            <p class="font-body text-xs text-slate-400">Tus transacciones se almacenan localmente con cifrado AES-256 hasta que se restaure la conexión.</p>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.font-headline {
  font-family: 'Manrope', sans-serif;
}
.font-body, .font-label {
  font-family: 'Inter', sans-serif;
}
</style>

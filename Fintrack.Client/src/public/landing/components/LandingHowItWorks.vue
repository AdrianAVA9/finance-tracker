<template>
  <section class="w-full py-24 px-4 bg-background-dark relative" id="how-it-works">
    <div class="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-primary/5 via-transparent to-transparent"></div>
    <div class="max-w-6xl mx-auto relative z-10">
      <div class="text-center mb-16">
        <h2 class="font-heading text-3xl md:text-4xl font-bold text-white">De Recibo a Informe en Segundos</h2>
        <p class="text-slate-400 mt-4">Tres simples pasos para la claridad financiera.</p>
      </div>
      <div class="relative grid grid-cols-1 md:grid-cols-3 gap-8">
        <!-- Connector Line (Desktop) -->
        <div class="hidden md:block absolute top-12 left-[16%] right-[16%] h-[2px] bg-gradient-to-r from-surface-border via-primary/50 to-surface-border -z-10"></div>
        <!-- Step 1 -->
        <div class="flex flex-col items-center text-center gap-4 group">
          <div class="w-24 h-24 rounded-2xl bg-surface-dark border border-surface-border flex items-center justify-center shadow-lg group-hover:border-primary/50 transition-colors duration-300 relative">
            <div class="absolute -top-3 -right-3 w-8 h-8 rounded-full bg-surface-dark border border-surface-border flex items-center justify-center font-bold text-slate-500 text-sm">1</div>
            <span class="material-symbols-outlined text-4xl text-slate-300 group-hover:text-primary transition-colors">center_focus_strong</span>
          </div>
          <h3 class="text-xl font-bold text-white mt-2">Toma la Foto y Escanea</h3>
          <p class="text-slate-400 text-sm max-w-[250px]">Simplemente toma una foto de tu recibo o reenvía una factura por correo a tu dirección dedicada de CeroBase.</p>
        </div>
        <!-- Step 2 -->
        <div class="flex flex-col items-center text-center gap-4 group">
          <div class="w-24 h-24 rounded-2xl bg-surface-dark border border-surface-border flex items-center justify-center shadow-lg group-hover:border-primary/50 transition-colors duration-300 relative">
            <div class="absolute -top-3 -right-3 w-8 h-8 rounded-full bg-surface-dark border border-surface-border flex items-center justify-center font-bold text-slate-500 text-sm">2</div>
            <span class="material-symbols-outlined text-4xl text-slate-300 group-hover:text-accent transition-colors">auto_fix_high</span>
          </div>
          <h3 class="text-xl font-bold text-white mt-2">Análisis de IA</h3>
          <p class="text-slate-400 text-sm max-w-[250px]">Nuestro motor extrae el comercio, la fecha, los impuestos y el total. Categoriza automáticamente el gasto basándose en el historial.</p>
        </div>
        <!-- Step 3 -->
        <div class="flex flex-col items-center text-center gap-4 group">
          <div class="w-24 h-24 rounded-2xl bg-surface-dark border border-surface-border flex items-center justify-center shadow-lg group-hover:border-primary/50 transition-colors duration-300 relative">
            <div class="absolute -top-3 -right-3 w-8 h-8 rounded-full bg-surface-dark border border-surface-border flex items-center justify-center font-bold text-slate-500 text-sm">3</div>
            <span class="material-symbols-outlined text-4xl text-slate-300 group-hover:text-emerald-400 transition-colors">verified</span>
          </div>
          <h3 class="text-xl font-bold text-white mt-2">Revisa y Guarda</h3>
          <p class="text-slate-400 text-sm max-w-[250px]">Verifica rápidamente los detalles (o deja que el Auto-Piloto lo haga) y sincroniza los datos con tu tablero principal al instante.</p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

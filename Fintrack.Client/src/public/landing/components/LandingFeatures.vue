<template>
  <section class="w-full py-20 px-4 bg-[#111111]" id="features">
    <div class="max-w-7xl mx-auto flex flex-col gap-10">
      <div class="text-center max-w-2xl mx-auto">
        <h2 class="font-heading text-3xl md:text-4xl font-bold text-white mb-4">Todo lo que necesitas para dominar tu dinero</h2>
        <p class="text-slate-400">Nuestra cuadrícula de herramientas te pone el control con precisión de IA.</p>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4 auto-rows-[minmax(280px,auto)]">
        <!-- Feature 1: Large Scan -->
        <div class="md:col-span-2 relative group overflow-hidden rounded-2xl border border-surface-border bg-surface-dark p-8 flex flex-col justify-between hover:border-primary/50 transition-colors duration-300">
          <div class="relative z-10 flex flex-col gap-2">
            <div class="w-12 h-12 rounded-lg bg-primary/20 flex items-center justify-center text-primary mb-2">
              <span class="material-symbols-outlined text-2xl">document_scanner</span>
            </div>
            <h3 class="text-2xl font-bold text-white">Escaneo de Facturas con IA</h3>
            <p class="text-slate-400 max-w-md">Convierte recibos en datos al instante. Nuestro motor inteligente captura cada detalle automáticamente, reconociendo proveedores, fechas y montos con un 99.8% de precisión.</p>
          </div>
          <!-- Visualization: Scanning Effect -->
          <div class="absolute top-0 right-0 h-full w-1/3 min-w-[200px] bg-gradient-to-l from-black/20 to-transparent pointer-events-none"></div>
          <div class="absolute right-0 bottom-0 w-[300px] h-[200px] opacity-30 md:opacity-50">
            <div class="absolute inset-0 bg-cover bg-center rounded-tl-2xl border-t border-l border-surface-border" data-alt="Wireframe receipt representation" style="background-image: url('https://lh3.googleusercontent.com/aida-public/AB6AXuD-nhksMo-qS-j48FKsV2oGxLJlG5ZvnjQrEf_DIv5LDPf5tigvAvFlEjm2M6lc4YGHCn1ssQ1i_gZTR6_41iu1wH5jbl244c9HkaThMkQseJtBai0NPyujlIWQ6YnPMMa8kxYldaqrHPsYsy77ZB22uU0z-ejBZJP66KUJ8tJpGauaXeFWZhKk0MnLaFqEDC54w6SVfhPAaYe19dOkfj25f1KFUJDrH5QwIlAiOR52tQjOVCHp3COQb8fOAmvZ4rtXSiVH7ndl8hb5')"></div>
            <!-- Scanning Line -->
            <div class="absolute top-0 left-0 w-full h-[2px] bg-accent shadow-[0_0_15px_#00C4CC] animate-[scan_3s_ease-in-out_infinite] scan-line"></div>
          </div>
        </div>
        <!-- Feature 2: Encryption -->
        <div class="md:col-span-1 relative group overflow-hidden rounded-2xl border border-surface-border bg-surface-dark p-8 flex flex-col">
          <div class="w-12 h-12 rounded-lg bg-emerald-500/20 flex items-center justify-center text-emerald-400 mb-6">
            <span class="material-symbols-outlined text-2xl">shield_lock</span>
          </div>
          <h3 class="text-xl font-bold text-white mb-2">Encriptación de Nivel Bancario</h3>
          <p class="text-slate-400 text-sm">Tus datos están protegidos con encriptación AES-256. Nunca vendemos tu información personal.</p>
          <div class="mt-auto pt-6 flex justify-center">
            <span class="material-symbols-outlined text-6xl text-emerald-500/10 group-hover:text-emerald-500/20 transition-colors">lock</span>
          </div>
        </div>
        <!-- Feature 3: Budget Tracking -->
        <div class="md:col-span-1 relative group overflow-hidden rounded-2xl border border-surface-border bg-surface-dark p-8 flex flex-col">
          <div class="w-12 h-12 rounded-lg bg-orange-500/20 flex items-center justify-center text-orange-400 mb-6">
            <span class="material-symbols-outlined text-2xl">pie_chart</span>
          </div>
          <h3 class="text-xl font-bold text-white mb-2">Presupuestos en Tiempo Real</h3>
          <p class="text-slate-400 text-sm">Visualiza tus gastos frente a tus metas con gráficos dinámicos que se actualizan al instante.</p>
          <div class="mt-6 h-24 w-full flex items-end justify-between gap-1 opacity-50">
            <div class="w-full bg-orange-500/20 h-[40%] rounded-t"></div>
            <div class="w-full bg-orange-500/40 h-[70%] rounded-t"></div>
            <div class="w-full bg-orange-500 h-[50%] rounded-t"></div>
            <div class="w-full bg-orange-500/20 h-[30%] rounded-t"></div>
          </div>
        </div>
        <!-- Feature 4: Integration -->
        <div class="md:col-span-2 relative group overflow-hidden rounded-2xl border border-surface-border bg-surface-dark p-8 flex flex-col md:flex-row items-center gap-8">
          <div class="flex-1">
            <div class="w-12 h-12 rounded-lg bg-purple-500/20 flex items-center justify-center text-purple-400 mb-6">
              <span class="material-symbols-outlined text-2xl">hub</span>
            </div>
            <h3 class="text-xl font-bold text-white mb-2">Conéctate con más de 5,000 Bancos</h3>
            <p class="text-slate-400">Sincroniza perfectamente tus cuentas para una visión financiera holística.</p>
          </div>
          <div class="flex-1 w-full">
            <!-- Bank Logo Grid Abstraction -->
            <div class="grid grid-cols-3 gap-3 opacity-60" data-alt="Grid of abstract bank logos">
              <div class="h-10 bg-white/5 rounded flex items-center justify-center"><span class="text-[10px] text-white/50">CHASE</span></div>
              <div class="h-10 bg-white/5 rounded flex items-center justify-center"><span class="text-[10px] text-white/50">BOA</span></div>
              <div class="h-10 bg-white/5 rounded flex items-center justify-center"><span class="text-[10px] text-white/50">WELLS</span></div>
              <div class="h-10 bg-white/5 rounded flex items-center justify-center"><span class="text-[10px] text-white/50">CITI</span></div>
              <div class="h-10 bg-white/5 rounded flex items-center justify-center"><span class="text-[10px] text-white/50">AMEX</span></div>
              <div class="h-10 bg-white/5 rounded flex items-center justify-center"><span class="text-[10px] text-white/50">+4995</span></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
@keyframes scan {
  0%, 100% { top: 0%; opacity: 0; }
  10% { opacity: 1; }
  90% { opacity: 1; }
  50% { top: 100%; }
}
.scan-line {
    animation: scan 3s ease-in-out infinite;
}
</style>

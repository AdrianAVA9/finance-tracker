<template>
  <section class="relative w-full py-20 lg:py-32 px-4 overflow-hidden">
    <!-- Background Glows -->
    <div class="absolute top-0 left-1/2 -translate-x-1/2 w-[1000px] h-[600px] bg-primary/10 rounded-full blur-[120px] -z-10 pointer-events-none mix-blend-screen"></div>
    <div class="max-w-7xl mx-auto grid lg:grid-cols-2 gap-12 lg:gap-20 items-center">
      <div class="flex flex-col gap-6 text-center lg:text-left z-10">
        <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-surface-dark border border-surface-border w-fit mx-auto lg:mx-0">
          <span class="w-2 h-2 rounded-full bg-accent animate-pulse"></span>
          <span class="text-xs font-medium text-accent uppercase tracking-wider">v2.0 Ya disponible</span>
        </div>
        <h1 class="font-heading text-5xl sm:text-6xl lg:text-7xl font-bold leading-[1.1] tracking-tight text-white">
          Tus Finanzas, <br/>
          <span class="text-transparent bg-clip-text bg-gradient-to-r from-white via-slate-200 to-slate-400">Automatizadas por IA.</span>
        </h1>
        <p class="text-lg text-slate-400 max-w-xl mx-auto lg:mx-0 leading-relaxed">
          Deja el registro manual. Deja que nuestro motor inteligente capture recibos, categorice gastos y asegure tu futuro con precisión bancaria.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start pt-4">
          <a href="/auth/register" class="bg-primary hover:bg-primary-hover text-white text-base font-bold h-12 px-8 rounded-xl transition-all shadow-lg hover:shadow-primary/25 flex items-center justify-center gap-2 group">
            Empieza Gratis
            <span class="material-symbols-outlined text-lg group-hover:translate-x-1 transition-transform">arrow_forward</span>
          </a>
          <button class="bg-surface-dark hover:bg-[#252525] border border-surface-border text-white text-base font-medium h-12 px-8 rounded-xl transition-all flex items-center justify-center gap-2">
            <span class="material-symbols-outlined text-lg text-slate-400">play_circle</span>
            Ver Demo
          </button>
        </div>
        <div class="pt-8 flex items-center justify-center lg:justify-start gap-6 opacity-60 grayscale hover:grayscale-0 transition-all duration-500">
          <!-- Logos replaced with text for simplicity/compliance -->
          <span class="text-sm font-semibold text-slate-500">CONFIADO POR EQUIPOS EN</span>
          <div class="flex gap-4 text-slate-400 font-heading font-bold">
            <span>ACME</span>
            <span>CORP</span>
            <span>GLOBEX</span>
          </div>
        </div>
      </div>
      <!-- Hero Visual -->
      <div class="relative w-full aspect-square max-w-[600px] mx-auto lg:mr-0 perspective-1000">
        <!-- Main Card Visual -->
        <div class="relative z-10 w-full h-full bg-surface-dark/50 backdrop-blur-sm border border-surface-border rounded-2xl overflow-hidden shadow-2xl animate-float">
          <div class="absolute inset-0 bg-gradient-to-br from-primary/10 via-transparent to-transparent opacity-50"></div>
          <!-- Mock Interface Header -->
          <div class="flex items-center justify-between px-6 py-4 border-b border-surface-border bg-[#151515]">
            <div class="flex gap-2">
              <div class="w-3 h-3 rounded-full bg-red-500/50"></div>
              <div class="w-3 h-3 rounded-full bg-yellow-500/50"></div>
              <div class="w-3 h-3 rounded-full bg-green-500/50"></div>
            </div>
            <div class="text-[10px] font-mono text-slate-500 uppercase">Panel • En Vivo</div>
          </div>
          <!-- Abstract Data Visualization -->
          <div class="p-8 flex flex-col gap-6 h-full" data-alt="Abstract interface showing a spending graph with an upward trend and expense categories">
            <div class="flex justify-between items-end">
              <div>
                <div class="text-sm text-slate-400">Balance Total</div>
                <div class="text-3xl font-heading font-bold text-white">₡124,592.00</div>
              </div>
              <div class="text-accent text-sm font-bold flex items-center gap-1 bg-accent/10 px-2 py-1 rounded">
                <span class="material-symbols-outlined text-sm">trending_up</span> +12.4%
              </div>
            </div>
            <!-- Fake Graph -->
            <div class="h-32 w-full flex items-end gap-2 mt-4">
              <div class="w-full bg-primary/20 rounded-t-sm h-[40%] hover:bg-primary/40 transition-all duration-500 cursor-pointer group relative">
                <div class="absolute -top-8 left-1/2 -translate-x-1/2 bg-surface-dark border border-surface-border px-2 py-1 text-xs rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap z-20">Lun: ₡420</div>
              </div>
              <div class="w-full bg-primary/20 rounded-t-sm h-[60%] hover:bg-primary/40 transition-all duration-500 cursor-pointer group relative">
                <div class="absolute -top-8 left-1/2 -translate-x-1/2 bg-surface-dark border border-surface-border px-2 py-1 text-xs rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap z-20">Mar: ₡640</div>
              </div>
              <div class="w-full bg-primary/20 rounded-t-sm h-[30%] hover:bg-primary/40 transition-all duration-500 cursor-pointer group relative">
                <div class="absolute -top-8 left-1/2 -translate-x-1/2 bg-surface-dark border border-surface-border px-2 py-1 text-xs rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap z-20">Mié: ₡310</div>
              </div>
              <div class="w-full bg-primary/20 rounded-t-sm h-[80%] hover:bg-primary/40 transition-all duration-500 cursor-pointer group relative">
                <div class="absolute -top-8 left-1/2 -translate-x-1/2 bg-surface-dark border border-surface-border px-2 py-1 text-xs rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap z-20">Jue: ₡890</div>
              </div>
              <div class="w-full bg-primary rounded-t-sm h-[55%] relative shadow-[0_0_15px_rgba(10,125,127,0.5)]"></div>
              <div class="w-full bg-surface-border/30 rounded-t-sm h-[0%]"></div> <!-- Future -->
            </div>
            <!-- List Items -->
            <div class="space-y-3 mt-4">
              <div class="flex items-center gap-3 p-3 rounded-lg bg-surface-border/20 border border-surface-border/50">
                <div class="p-2 rounded bg-orange-500/20 text-orange-400">
                  <span class="material-symbols-outlined text-lg">receipt_long</span>
                </div>
                <div class="flex-1 min-w-0">
                  <div class="text-sm font-medium text-white truncate">Infraestructura AWS</div>
                  <div class="text-xs text-slate-500">Servicios en la Nube</div>
                </div>
                <div class="text-sm font-bold text-white">-₡2,400.00</div>
              </div>
            </div>
          </div>
        </div>
        <!-- Floating Badge -->
        <div class="absolute -bottom-6 -left-6 z-20 bg-surface-dark border border-surface-border p-4 rounded-xl shadow-xl flex items-center gap-3 animate-bounce-slow">
          <div class="relative w-10 h-10 flex items-center justify-center rounded-full bg-accent/20 text-accent">
            <span class="material-symbols-outlined">auto_awesome</span>
          </div>
          <div>
            <div class="text-xs text-slate-400 uppercase font-bold tracking-wider">Análisis IA</div>
            <div class="text-sm font-medium text-white">Suscripción optimizada</div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}
.animate-float {
  animation: float 6s ease-in-out infinite;
}
.animate-bounce-slow {
  animation: float 4s ease-in-out infinite reverse;
}
.perspective-1000 {
  perspective: 1000px;
}
</style>

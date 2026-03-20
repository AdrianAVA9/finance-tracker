<template>
  <div>
    <!-- CTA Footer -->
    <section class="w-full py-20 px-4 bg-gradient-to-b from-background-dark to-[#051111] border-t border-surface-border">
      <div class="max-w-4xl mx-auto text-center flex flex-col items-center gap-8">
        <h2 class="font-heading text-4xl md:text-5xl font-bold text-white">¿Listo para automatizar tu futuro?</h2>
        <p class="text-slate-400 text-lg max-w-xl">Únete a más de 50,000 usuarios que ahorran un promedio de 12 horas al mes en gestión financiera.</p>
        <button class="bg-primary hover:bg-primary-hover text-white text-lg font-bold h-14 px-10 rounded-xl transition-all shadow-lg hover:shadow-primary/40 flex items-center justify-center gap-3">
          Empieza Ahora
          <span class="material-symbols-outlined">rocket_launch</span>
        </button>
        <p class="text-xs text-slate-500">No se requiere tarjeta de crédito para la prueba de 14 días.</p>
      </div>
    </section>

    <!-- Main Footer -->
    <footer class="bg-[#050505] border-t border-surface-border pt-16 pb-8 px-4 text-sm">
      <div class="max-w-7xl mx-auto">
        <div class="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-8 mb-12">
          <div class="col-span-2 lg:col-span-2">
            <div class="flex items-center gap-2 mb-4">
              <div class="w-6 h-6 rounded bg-primary flex items-center justify-center text-white">
                <span class="material-symbols-outlined text-sm">account_balance_wallet</span>
              </div>
              <span class="font-heading font-bold text-lg text-white">Fintrack</span>
            </div>
            <p class="text-slate-500 max-w-xs mb-6">
              El rastreador de gastos más inteligente del mundo. Construido para profesionales modernos que valoran su tiempo.
            </p>
            <div class="flex gap-4">
              <a class="text-slate-400 hover:text-white transition-colors" href="#"><span class="sr-only">Twitter</span>
                <svg class="h-5 w-5" fill="currentColor" viewbox="0 0 24 24"><path d="M8.29 20.251c7.547 0 11.675-6.253 11.675-11.675 0-.178 0-.355-.012-.53A8.348 8.348 0 0022 5.92a8.19 8.19 0 01-2.357.646 4.118 4.118 0 001.804-2.27 8.224 8.224 0 01-2.605.996 4.107 4.107 0 00-6.993 3.743 11.65 11.65 0 01-8.457-4.287 4.106 4.106 0 001.27 5.477A4.072 4.072 0 012.8 9.713v.052a4.105 4.105 0 003.292 4.022 4.095 4.095 0 01-1.853.07 4.108 4.108 0 003.834 2.85A8.233 8.233 0 012 18.407a11.616 11.616 0 006.29 1.84"></path></svg>
              </a>
              <a class="text-slate-400 hover:text-white transition-colors" href="#"><span class="sr-only">GitHub</span>
                <svg class="h-5 w-5" fill="currentColor" viewbox="0 0 24 24"><path clip-rule="evenodd" d="M12 2C6.477 2 2 6.484 2 12.017c0 4.425 2.865 8.18 6.839 9.504.5.092.682-.217.682-.483 0-.237-.008-.868-.013-1.703-2.782.605-3.369-1.343-3.369-1.343-.454-1.158-1.11-1.466-1.11-1.466-.908-.62.069-.608.069-.608 1.003.07 1.531 1.032 1.531 1.032.892 1.53 2.341 1.088 2.91.832.092-.647.35-1.088.636-1.338-2.22-.253-4.555-1.113-4.555-4.951 0-1.093.39-1.988 1.029-2.688-.103-.253-.446-1.272.098-2.65 0 0 .84-.27 2.75 1.026A9.564 9.564 0 0112 6.844c.85.004 1.705.115 2.504.337 1.909-1.296 2.747-1.027 2.747-1.027.546 1.379.202 2.398.1 2.651.64.7 1.028 1.595 1.028 2.688 0 3.848-2.339 4.695-4.566 4.943.359.309.678.92.678 1.855 0 1.338-.012 2.419-.012 2.747 0 .268.18.58.688.482A10.019 10.019 0 0022 12.017C22 6.484 17.522 2 12 2z" fill-rule="evenodd"></path></svg>
              </a>
            </div>
          </div>
          <div class="flex flex-col gap-3">
            <h4 class="font-bold text-white mb-2">Producto</h4>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Funcionalidades</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Precios</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">API</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Registro de Cambios</a>
          </div>
          <div class="flex flex-col gap-3">
            <h4 class="font-bold text-white mb-2">Empresa</h4>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Acerca de</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Carreras</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Blog</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Contacto</a>
          </div>
          <div class="flex flex-col gap-3">
            <h4 class="font-bold text-white mb-2">Legal</h4>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Política de Privacidad</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Términos de Servicio</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Cumplimiento de GDPR</a>
            <a class="text-slate-400 hover:text-primary transition-colors" href="#">Seguridad</a>
          </div>
        </div>
        <div class="border-t border-surface-border pt-8 flex flex-col md:flex-row justify-between items-center gap-4">
          <p class="text-slate-600 text-xs">© 2023 Fintrack Inc. Todos los derechos reservados.</p>
          <div class="flex items-center gap-2">
            <div class="w-2 h-2 rounded-full bg-green-500"></div>
            <span class="text-slate-500 text-xs">Todos los sistemas operativos</span>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
</script>

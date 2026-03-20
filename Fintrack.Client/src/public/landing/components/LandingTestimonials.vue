<template>
  <section class="w-full py-20 px-4 bg-[#0f0f0f]" id="testimonials">
    <div class="max-w-7xl mx-auto">
      <h2 class="font-heading text-3xl font-bold text-white text-center mb-12">Confiado por profesionales independientes</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- Card 1 -->
        <div class="p-6 rounded-xl bg-surface-dark border border-surface-border hover:border-surface-border/80 transition-all">
          <div class="flex gap-1 text-accent mb-4">
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
          </div>
          <p class="text-slate-300 mb-6 leading-relaxed">"El escaneo con IA es realmente mágico. Solía pasar horas los domingos ingresando datos. Ahora toma segundos."</p>
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-full bg-slate-700 bg-cover bg-center" data-alt="Portrait of Alex M" style="background-image: url('https://lh3.googleusercontent.com/aida-public/AB6AXuAhEutyb1T-AN5QoB4GmFRAs5Aq2WsZbXeQodo9HIhzgEoi6WpZHevLm6s5LVkR37IdbgPwLdursFnnxsg2ekJxto4Y-gb5gbwpyWbbTITih12DBp0Q4TNfHLVPpXWMp0U6K63Akb7QWM_R4ML-6ETdVMHOK0hqqQdNsq48JqqP3tVeYzZLtLt_zE63tN7ef2hqYeiRhZ9Q5UoG4qVpC9-mMh-RUTlJ3f1viE74TbhSBtM24EyZocEXnKEjpvL1ythinQ-yMt7aNdrN')"></div>
            <div>
              <div class="text-white font-bold text-sm">Alex M.</div>
              <div class="text-slate-500 text-xs">Diseñador UX</div>
            </div>
          </div>
        </div>
        <!-- Card 2 -->
        <div class="p-6 rounded-xl bg-surface-dark border border-surface-border hover:border-surface-border/80 transition-all">
          <div class="flex gap-1 text-accent mb-4">
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
          </div>
          <p class="text-slate-300 mb-6 leading-relaxed">"La seguridad era mi principal preocupación. Los estándares de encriptación aquí son mejores que los de la app de mi banco."</p>
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-full bg-slate-700 bg-cover bg-center" data-alt="Portrait of Sarah L" style="background-image: url('https://lh3.googleusercontent.com/aida-public/AB6AXuAR2tKOM88mgBlBYKP8mWO6MctD7Kk7BexQTkQDia1N5TBDNUQ9_-smC03y4jsDU6KgmS_MuRgtShb6Yzk1ECgTZKnrIMZkYpYgNhTG0namTywC_F2TzJv01ggnuH21jafagjaRWLPyZqBYMBGAtMnbtL1OZMF9W03DQhOJYVdlSx5WCng5Kh7dsZQcI020bYIe8NF86JD_ubUo5EsgcJAdqgQMyxsaMUj_rGhYNrffvs6bgRIDOY2C8mFw7t3Ht3nsB1Wfqp7amF2K')"></div>
            <div>
              <div class="text-white font-bold text-sm">Sarah L.</div>
              <div class="text-slate-500 text-xs">Dueña de Pequeño Negocio</div>
            </div>
          </div>
        </div>
        <!-- Card 3 -->
        <div class="p-6 rounded-xl bg-surface-dark border border-surface-border hover:border-surface-border/80 transition-all">
          <div class="flex gap-1 text-accent mb-4">
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
            <span class="material-symbols-outlined filled-icon text-lg">star</span>
          </div>
          <p class="text-slate-300 mb-6 leading-relaxed">"Finalmente, una herramienta de presupuesto que no se siente como una hoja de cálculo de 1999. La interfaz es impresionante."</p>
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-full bg-slate-700 bg-cover bg-center" data-alt="Portrait of David K" style="background-image: url('https://lh3.googleusercontent.com/aida-public/AB6AXuAvr3C36Mi1cRwLWXEuwv-H79YFhkWpTgm_nsAP7RzARWyzeKqTwIrhTkqHvAP21Q8GpGEGFvwkjBwJYvwLR2xz09op3Z139eSnCnClmdQtPK6lWpqDDl98khsL0jdeQaHdHWpL5vwDt3TijKTOVMvHB6a7nYuNw9I3GxuNNPPVVPOlQUdgH-TmbuzW1zb4xjilO82sqWi8_IUnbuIjeBbkym-r7Snx8kbTO15G64tw2qO5O2SfPZQvD3GGtJZ_tZFln3A2QHXE-Ty5')"></div>
            <div>
              <div class="text-white font-bold text-sm">David K.</div>
              <div class="text-slate-500 text-xs">Ingeniero de Software</div>
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
.filled-icon {
  font-variation-settings: 'FILL' 1, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>

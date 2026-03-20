<template>
  <header class="fixed top-0 left-0 right-0 z-50 border-b border-surface-border bg-[#121212]/80 backdrop-blur-md">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16 sm:h-20">
        <!-- Logo -->
        <div class="flex items-center gap-3">
          <div class="flex items-center justify-center w-8 h-8 rounded-lg bg-gradient-to-br from-primary to-accent text-white">
            <span class="material-symbols-outlined text-lg">account_balance_wallet</span>
          </div>
          <span class="font-heading font-bold text-xl tracking-tight text-white">Fintrack</span>
        </div>
        <!-- Desktop Nav -->
        <nav class="hidden md:flex gap-8">
          <a class="text-sm font-medium text-slate-300 hover:text-white transition-colors" href="#features">Funcionalidades</a>
          <a class="text-sm font-medium text-slate-300 hover:text-white transition-colors" href="#how-it-works">Cómo funciona</a>
          <a class="text-sm font-medium text-slate-300 hover:text-white transition-colors" href="#testimonials">Historias</a>
        </nav>
        <!-- CTA -->
        <div class="flex items-center gap-4">
          <a class="hidden sm:block text-sm font-medium text-white hover:text-accent transition-colors" href="/auth/login">Iniciar Sesión</a>
          <a href="/auth/register" class="bg-primary hover:bg-primary-hover text-white text-sm font-bold py-2.5 px-5 rounded-lg transition-all duration-300 shadow-[0_0_15px_-3px_rgba(10,125,127,0.4)] hover:shadow-[0_0_20px_-3px_rgba(10,125,127,0.6)]">
            Empezar
          </a>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
</script>

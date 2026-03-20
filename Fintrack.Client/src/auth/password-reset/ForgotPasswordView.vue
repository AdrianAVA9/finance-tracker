<script setup lang="ts">
import { ref } from 'vue';
import { usePasswordReset } from '@/composables/usePasswordReset';

const { isLoading, error, success, sendResetLink } = usePasswordReset();
const email = ref('');

const handleSubmit = async () => {
  await sendResetLink(email.value);
};
</script>

<template>
  <div class="bg-[#0B0E11] text-white min-h-screen flex flex-col font-display">
    <!-- Main Container -->
    <div class="flex flex-1 items-center justify-center px-4">
      <!-- Centered Reset Card -->
      <div class="w-full max-w-md bg-slate-900/40 border border-slate-800 p-8 rounded-xl shadow-2xl">

        <!-- Success State -->
        <div v-if="success" class="flex flex-col items-center gap-4 py-4">
          <div class="bg-emerald-500/20 p-4 rounded-full">
            <span class="material-symbols-outlined text-emerald-400 text-4xl">mark_email_read</span>
          </div>
          <h2 class="text-slate-100 text-xl font-bold">Revisa tu bandeja de entrada</h2>
          <p class="text-slate-400 text-sm text-center">
            Si existe una cuenta con <span class="text-white font-medium">{{ email }}</span>, se ha enviado un enlace de recuperación.
          </p>
          <router-link
            to="/auth/login"
            class="mt-4 text-sm text-[#1e6a7b] hover:text-[#2a8fa8] transition-colors font-medium inline-flex items-center gap-2"
          >
            <span class="material-symbols-outlined text-lg">arrow_back</span>
            Volver al Inicio de Sesión
          </router-link>
        </div>

        <!-- Form State -->
        <template v-else>
          <!-- Logo/Brand Section -->
          <div class="flex flex-col items-center mb-8">
            <div class="bg-[#1e6a7b]/20 p-3 rounded-full mb-4">
              <span class="material-symbols-outlined text-[#1e6a7b] text-3xl">shield_lock</span>
            </div>
            <h1 class="text-slate-100 text-2xl font-bold tracking-tight">Fintrack</h1>
            <p class="text-slate-400 text-sm mt-2 text-center">Ingresa tu correo electrónico para recibir un enlace de recuperación.</p>
          </div>

          <!-- Error Message -->
          <p v-if="error" class="mb-4 text-red-400 text-sm font-medium text-center bg-red-500/10 border border-red-500/30 rounded-lg px-4 py-3">
            {{ error }}
          </p>

          <!-- Form -->
          <form @submit.prevent="handleSubmit" class="space-y-6">
            <div class="space-y-2">
              <label class="text-slate-200 text-sm font-medium block ml-1" for="reset-email">
                Correo Electrónico
              </label>
              <div class="relative">
                <span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 text-xl">mail</span>
                <input
                  v-model="email"
                  id="reset-email"
                  type="email"
                  placeholder="name@company.com"
                  required
                  class="w-full pl-12 pr-4 py-3.5 bg-slate-950 border border-slate-800 rounded-lg text-slate-100 placeholder:text-slate-600 focus:ring-2 focus:ring-[#1e6a7b]/50 focus:border-[#1e6a7b] outline-none transition-all"
                />
              </div>
            </div>

            <button
              type="submit"
              :disabled="isLoading"
              class="w-full bg-[#1e6a7b] hover:bg-[#1e6a7b]/90 text-white font-bold py-3.5 rounded-lg shadow-lg shadow-[#1e6a7b]/20 transition-all flex items-center justify-center gap-2 group disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="isLoading">Enviando...</span>
              <template v-else>
                <span>Enviar Enlace</span>
                <span class="material-symbols-outlined text-xl group-hover:translate-x-1 transition-transform">arrow_forward</span>
              </template>
            </button>
          </form>

          <!-- Navigation Links -->
          <div class="mt-8 pt-6 border-t border-slate-800 text-center">
            <router-link
              to="/auth/login"
              class="text-slate-400 hover:text-[#1e6a7b] text-sm font-medium transition-colors inline-flex items-center gap-2"
            >
              <span class="material-symbols-outlined text-lg">arrow_back</span>
              Volver al Inicio de Sesión
            </router-link>
          </div>
        </template>

      </div>
    </div>

    <!-- Footer Security Note -->
    <footer class="py-8 text-center">
      <div class="flex items-center justify-center gap-2 text-slate-600 text-xs font-medium uppercase tracking-widest">
        <span class="material-symbols-outlined text-sm">verified_user</span>
        Recuperación Encriptada de Extremo a Extremo
      </div>
    </footer>
  </div>
</template>

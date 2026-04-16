<script setup lang="ts">
import { ref } from 'vue';
import { usePasswordReset } from '@/composables/usePasswordReset';
import logo from '@/assets/logo.png';
import ForgotPasswordRequestCard from '@/auth/password-reset/components/ForgotPasswordRequestCard.vue';
import ForgotPasswordSuccessCard from '@/auth/password-reset/components/ForgotPasswordSuccessCard.vue';
import PasswordResetHeader from '@/auth/password-reset/components/PasswordResetHeader.vue';

const { isLoading, error, success, sendResetLink } = usePasswordReset();
const email = ref('');

const handleSubmit = async () => {
  await sendResetLink(email.value);
};
</script>

<template>
  <div class="min-h-screen bg-[#0B0E11] font-display text-white">
    <div class="flex min-h-screen items-center justify-center px-4 py-8">
      <div class="w-full max-w-md space-y-8">
        <PasswordResetHeader
          :logo-src="logo"
          title="CeroBase"
          subtitle="Ingresa tu correo electrónico para recibir un enlace de recuperación."
        />

        <ForgotPasswordSuccessCard v-if="success" :email="email" />
        <template v-else>
          <ForgotPasswordRequestCard
            :email="email"
            :is-loading="isLoading"
            :error="error"
            @update:email="email = $event"
            @submit="handleSubmit"
          />

          <div class="border-t border-slate-800 pt-6 text-center">
            <router-link
              to="/auth/login"
              class="inline-flex items-center gap-2 text-sm font-medium text-slate-400 transition-colors hover:text-[#1e6a7b]"
            >
              <span class="material-symbols-outlined text-lg">arrow_back</span>
              Volver al Inicio de Sesión
            </router-link>
          </div>
        </template>
      </div>
    </div>

    <footer class="py-8 text-center">
      <div class="flex items-center justify-center gap-2 text-xs font-medium uppercase tracking-widest text-slate-600">
        <span class="material-symbols-outlined text-sm">verified_user</span>
        Recuperación encriptada de extremo a extremo
      </div>
    </footer>
  </div>
</template>

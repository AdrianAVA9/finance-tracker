<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { usePasswordReset } from '@/composables/usePasswordReset';
import logo from '@/assets/logo.png';
import PasswordResetHeader from '@/auth/password-reset/components/PasswordResetHeader.vue';
import ResetPasswordFormCard from '@/auth/password-reset/components/ResetPasswordFormCard.vue';
import ResetPasswordSuccessCard from '@/auth/password-reset/components/ResetPasswordSuccessCard.vue';

const route = useRoute();
const router = useRouter();
const { isLoading, error, success, resetPassword } = usePasswordReset();

// The backend sends `?email=...&resetCode=...` in the reset link
const email = ref((route.query.email as string) ?? '');
const resetCode = ref((route.query.resetCode as string) ?? '');

const newPassword = ref('');
const confirmPassword = ref('');

const passwordsMatch = computed(() => newPassword.value === confirmPassword.value);
const canSubmit = computed(() => newPassword.value.length >= 6 && passwordsMatch.value);

const validationError = computed(() => {
  if (confirmPassword.value && !passwordsMatch.value) return 'Las contraseñas no coinciden.';
  return '';
});

const handleSubmit = async () => {
  if (!canSubmit.value) return;
  const ok = await resetPassword(email.value, resetCode.value, newPassword.value);
  if (ok) {
    setTimeout(() => router.push('/auth/login'), 2500);
  }
};
</script>

<template>
  <div class="relative min-h-screen bg-[#0B0E11] p-4 font-display text-white">
    <div class="pointer-events-none fixed bottom-8 flex select-none gap-8 opacity-[0.07]">
      <span class="material-symbols-outlined text-6xl text-[#1e6a7b]">shield</span>
      <span class="material-symbols-outlined text-6xl text-[#1e6a7b]">encrypted</span>
      <span class="material-symbols-outlined text-6xl text-[#1e6a7b]">key</span>
    </div>

    <div class="mx-auto flex min-h-screen w-full max-w-md items-center justify-center">
      <div class="w-full space-y-8">
        <PasswordResetHeader
          :logo-src="logo"
          logo-class="h-12 w-12 rounded-xl overflow-hidden shadow-lg shadow-[#1e6a7b]/10"
          title="Asegura tu Cuenta"
          subtitle="Elige una contraseña segura y única para proteger tus datos."
        />

        <ResetPasswordSuccessCard v-if="success" />
        <ResetPasswordFormCard
          v-else
          :new-password="newPassword"
          :confirm-password="confirmPassword"
          :is-loading="isLoading"
          :error="error"
          :can-submit="canSubmit"
          :validation-error="validationError"
          @update:new-password="newPassword = $event"
          @update:confirm-password="confirmPassword = $event"
          @submit="handleSubmit"
        />

        <div class="text-center">
          <router-link
            to="/auth/login"
            class="inline-flex items-center gap-2 text-sm font-medium text-slate-400 transition-colors hover:text-[#1e6a7b]"
          >
            <span class="material-symbols-outlined text-lg">arrow_back</span>
            Volver al Inicio de Sesión
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { usePasswordReset } from '@/composables/usePasswordReset';

const route = useRoute();
const router = useRouter();
const { isLoading, error, success, resetPassword } = usePasswordReset();

// The backend sends `?email=...&resetCode=...` in the reset link
const email = ref((route.query.email as string) ?? '');
const resetCode = ref((route.query.resetCode as string) ?? '');

const newPassword = ref('');
const confirmPassword = ref('');
const showPassword = ref(false);
const showConfirmPassword = ref(false);

const passwordsMatch = computed(() => newPassword.value === confirmPassword.value);
const canSubmit = computed(() => newPassword.value.length >= 6 && passwordsMatch.value);

const passwordStrength = computed(() => {
  const pw = newPassword.value;
  if (pw.length === 0) return { label: 'None', width: '0%', color: '#1e293b' };
  if (pw.length < 6)   return { label: 'Weak',   width: '25%', color: '#ef4444' };
  if (pw.length < 10)  return { label: 'Medium', width: '55%', color: '#f59e0b' };
  if (pw.match(/[A-Z]/) && pw.match(/[0-9]/) && pw.match(/[^a-zA-Z0-9]/))
                       return { label: 'Strong',  width: '100%', color: '#1e6a7b' };
  return              { label: 'Good',   width: '75%', color: '#22c55e' };
});

const validationError = computed(() => {
  if (confirmPassword.value && !passwordsMatch.value) return 'Passwords do not match.';
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
  <div class="bg-[#0B0E11] text-white min-h-screen flex flex-col items-center justify-center p-4 font-display relative">

    <!-- Decorative background icons -->
    <div class="fixed bottom-8 flex gap-8 opacity-[0.07] pointer-events-none select-none">
      <span class="material-symbols-outlined text-6xl text-[#1e6a7b]">shield</span>
      <span class="material-symbols-outlined text-6xl text-[#1e6a7b]">encrypted</span>
      <span class="material-symbols-outlined text-6xl text-[#1e6a7b]">key</span>
    </div>

    <div class="w-full max-w-md">

      <!-- Header -->
      <div class="mb-8 flex flex-col items-center gap-4">
        <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-[#1e6a7b]/10 text-[#1e6a7b]">
          <span class="material-symbols-outlined text-3xl">lock_reset</span>
        </div>
        <div class="text-center">
          <h1 class="text-2xl font-bold tracking-tight text-white">Secure Your Account</h1>
          <p class="mt-2 text-sm text-slate-400">Choose a strong, unique password to protect your data.</p>
        </div>
      </div>

      <!-- Success State -->
      <div v-if="success" class="bg-slate-900/50 border border-slate-800 p-8 rounded-xl shadow-2xl flex flex-col items-center gap-4">
        <div class="bg-emerald-500/20 p-4 rounded-full">
          <span class="material-symbols-outlined text-emerald-400 text-4xl">check_circle</span>
        </div>
        <h2 class="text-white text-xl font-bold">Password Updated!</h2>
        <p class="text-slate-400 text-sm text-center">Your password has been changed successfully. Redirecting you to login...</p>
      </div>

      <!-- Form -->
      <div v-else class="bg-slate-900/50 border border-slate-800 p-8 rounded-xl shadow-2xl backdrop-blur-sm">

        <!-- Error Banner -->
        <p v-if="error" class="mb-5 text-red-400 text-sm font-medium text-center bg-red-500/10 border border-red-500/30 rounded-lg px-4 py-3">
          {{ error }}
        </p>

        <form @submit.prevent="handleSubmit" class="space-y-6">

          <!-- New Password -->
          <div class="space-y-2">
            <label class="text-sm font-semibold text-slate-300 ml-1">New Password</label>
            <div class="relative flex items-center">
              <input
                v-model="newPassword"
                id="new-password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="Enter at least 6 characters"
                required
                class="block w-full rounded-xl border border-slate-700 bg-slate-800 text-white focus:border-[#1e6a7b] focus:ring-1 focus:ring-[#1e6a7b] py-3.5 px-4 pr-12 transition-all outline-none placeholder:text-slate-600"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute right-4 text-slate-400 hover:text-[#1e6a7b] transition-colors"
              >
                <span class="material-symbols-outlined text-xl">{{ showPassword ? 'visibility_off' : 'visibility' }}</span>
              </button>
            </div>
          </div>

          <!-- Password Strength -->
          <div class="space-y-3">
            <div class="flex items-center justify-between px-1">
              <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Password Strength</span>
              <span class="text-xs font-bold" :style="{ color: passwordStrength.color }">{{ passwordStrength.label }}</span>
            </div>
            <div class="h-1.5 w-full bg-slate-800 rounded-full overflow-hidden">
              <div
                class="h-full rounded-full transition-all duration-500"
                :style="{ width: passwordStrength.width, backgroundColor: passwordStrength.color }"
              />
            </div>
            <p class="text-[11px] text-slate-500 italic">
              Tip: Use a mix of uppercase, lowercase, numbers, and symbols.
            </p>
          </div>

          <!-- Confirm Password -->
          <div class="space-y-2">
            <label class="text-sm font-semibold text-slate-300 ml-1">Confirm Password</label>
            <div class="relative flex items-center">
              <input
                v-model="confirmPassword"
                id="confirm-password"
                :type="showConfirmPassword ? 'text' : 'password'"
                placeholder="Repeat new password"
                required
                :class="[
                  'block w-full rounded-xl border bg-slate-800 text-white py-3.5 px-4 pr-12 transition-all outline-none placeholder:text-slate-600 focus:ring-1',
                  confirmPassword && !passwordsMatch
                    ? 'border-red-500/70 focus:border-red-500 focus:ring-red-500/50'
                    : 'border-slate-700 focus:border-[#1e6a7b] focus:ring-[#1e6a7b]'
                ]"
              />
              <button
                type="button"
                @click="showConfirmPassword = !showConfirmPassword"
                class="absolute right-4 text-slate-400 hover:text-[#1e6a7b] transition-colors"
              >
                <span class="material-symbols-outlined text-xl">{{ showConfirmPassword ? 'visibility_off' : 'visibility' }}</span>
              </button>
            </div>
            <p v-if="validationError" class="text-red-400 text-xs ml-1">{{ validationError }}</p>
          </div>

          <!-- Submit -->
          <button
            type="submit"
            :disabled="isLoading || !canSubmit"
            class="w-full flex items-center justify-center gap-2 bg-[#1e6a7b] hover:bg-[#1e6a7b]/90 text-white font-bold py-4 rounded-xl transition-all shadow-lg shadow-[#1e6a7b]/20 active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="isLoading">Updating...</span>
            <template v-else>
              <span class="material-symbols-outlined text-xl">verified_user</span>
              Update Password
            </template>
          </button>
        </form>
      </div>

      <!-- Back link -->
      <div class="mt-8 text-center">
        <router-link
          to="/auth/login"
          class="inline-flex items-center gap-2 text-sm font-medium text-slate-400 hover:text-[#1e6a7b] transition-colors"
        >
          <span class="material-symbols-outlined text-lg">arrow_back</span>
          Back to Sign In
        </router-link>
      </div>
    </div>
  </div>
</template>

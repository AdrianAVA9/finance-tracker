<script setup lang="ts">
import logo from '@/assets/logo.png';
import LoginFormCard from '@/auth/login/components/LoginFormCard.vue';
import LoginMarketingPanel from '@/auth/login/components/LoginMarketingPanel.vue';
import { useLoginForm } from '@/auth/login/composables/useLoginForm';

const { email, password, loading, errorMsg, handleLogin } = useLoginForm();
const currentYear = new Date().getFullYear();
</script>

<template>
  <main class="bg-[#0e0e1b] text-white h-screen w-full overflow-hidden flex flex-row font-display relative z-50">
    <LoginMarketingPanel :logo-src="logo" />

    <div class="w-full lg:w-[40%] h-full bg-[#0F0F1A] flex flex-col overflow-y-auto">
      <div class="flex-1 flex flex-col justify-center items-center px-6 py-12 sm:px-12">
        <div class="lg:hidden mb-8 flex items-center gap-2">
          <img :src="logo" alt="CeroBase Logo" class="w-8 h-8 rounded-lg" />
          <span class="text-xl font-bold">CeroBase</span>
        </div>

        <LoginFormCard
          :email="email"
          :password="password"
          :loading="loading"
          :error-msg="errorMsg"
          @update:email="email = $event"
          @update:password="password = $event"
          @submit="handleLogin"
        />

        <p class="mt-8 text-xs text-slate-500 text-center">
          © {{ currentYear }} CeroBase Inc. Todos los derechos reservados.
        </p>
      </div>
    </div>
  </main>
</template>

<style scoped>
/* Custom scrollbar for subtle look */
::-webkit-scrollbar {
    width: 8px;
}

::-webkit-scrollbar-track {
    background: #0F0F1A;
}

::-webkit-scrollbar-thumb {
    background: #393960;
    border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
    background: #505080;
}
</style>

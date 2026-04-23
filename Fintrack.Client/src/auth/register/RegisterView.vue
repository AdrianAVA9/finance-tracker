<script setup lang="ts">
import logo from '@/assets/logo.png';
import RegisterFormCard from '@/auth/register/components/RegisterFormCard.vue';
import RegisterMarketingPanel from '@/auth/register/components/RegisterMarketingPanel.vue';
import { useRegisterForm } from '@/auth/register/composables/useRegisterForm';

const { fullName, email, password, terms, errorMsg, loading, passwordStrength, handleRegister } = useRegisterForm();
const currentYear = new Date().getFullYear();
</script>

<template>
  <main class="bg-[#0F0F1A] text-slate-100 font-display selection:bg-[#191971]/40 min-h-screen relative z-50">
    <div class="flex min-h-screen w-full flex-col lg:flex-row">
      <RegisterMarketingPanel :logo-src="logo" />

      <div class="flex w-full flex-col items-center justify-center px-6 py-12 lg:w-[40%] lg:px-12 bg-[#0F0F1A]">
        <div class="mb-10 flex items-center gap-2 lg:hidden">
          <img :src="logo" alt="CeroBase Logo" class="w-8 h-8 rounded-lg" />
          <span class="text-xl font-bold text-white">CeroBase</span>
        </div>

        <RegisterFormCard
          :full-name="fullName"
          :email="email"
          :password="password"
          :terms="terms"
          :loading="loading"
          :error-msg="errorMsg"
          :password-strength="passwordStrength"
          @update:full-name="fullName = $event"
          @update:email="email = $event"
          @update:password="password = $event"
          @update:terms="terms = $event"
          @submit="handleRegister"
        />

        <div class="mt-12 flex gap-6 text-[10px] font-bold uppercase tracking-widest text-slate-500">
          <a href="#" class="hover:text-[#AAAAAA] transition-colors">Seguridad</a>
          <a href="#" class="hover:text-[#AAAAAA] transition-colors">Soporte</a>
          <a href="#" class="hover:text-[#AAAAAA] transition-colors">Privacidad</a>
        </div>

        <p class="mt-5 text-xs text-slate-500 text-center">© {{ currentYear }} CeroBase Inc. Todos los derechos reservados.</p>
      </div>
    </div>
  </main>
</template>

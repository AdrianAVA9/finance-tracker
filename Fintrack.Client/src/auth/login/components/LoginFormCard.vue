<script setup lang="ts">
import { computed, ref } from 'vue';
import AppButton from '@/app/components/common/AppButton.vue';
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';

const props = defineProps<{
  email: string;
  password: string;
  loading: boolean;
  errorMsg: string;
}>();

const emit = defineEmits<{
  'update:email': [value: string];
  'update:password': [value: string];
  submit: [];
}>();

const showPassword = ref(false);
const passwordInputType = computed(() => (showPassword.value ? 'text' : 'password'));
const passwordIcon = computed(() => (showPassword.value ? 'visibility_off' : 'visibility'));

const onEmailInput = (event: Event) => {
  emit('update:email', (event.target as HTMLInputElement).value);
};

const onPasswordInput = (event: Event) => {
  emit('update:password', (event.target as HTMLInputElement).value);
};
</script>

<template>
  <SurfaceCard class="w-full max-w-[440px] bg-[#1A1A30] rounded-2xl shadow-2xl border border-[#393960]/30 relative overflow-hidden group !p-8">
    <div
      class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-transparent via-[#191971] to-transparent opacity-50 group-hover:opacity-100 transition-opacity duration-500"
    />

    <div class="mb-8">
      <h1 class="text-[32px] font-bold leading-tight text-white mb-2">Acceso Seguro</h1>
      <p class="text-[#9898c3] text-sm">Gestiona tu presupuesto con precisión.</p>
      <p v-if="errorMsg" class="mt-2 text-red-500 text-sm font-semibold">{{ errorMsg }}</p>
    </div>

    <form class="flex flex-col gap-5" @submit.prevent="emit('submit')">
      <div class="flex flex-col gap-2">
        <label for="email" class="text-white text-sm font-medium">Correo Electrónico</label>
        <input
          id="email"
          :value="props.email"
          type="email"
          placeholder="nombre@ejemplo.com"
          class="w-full h-12 rounded-lg bg-[#0F0F1A] border border-[#393960] text-white placeholder-[#9898c3] px-4 focus:outline-none focus:border-[#191971] focus:ring-1 focus:ring-[#191971] transition-all duration-200"
          @input="onEmailInput"
          required
        />
      </div>

      <div class="flex flex-col gap-2">
        <label for="password" class="text-white text-sm font-medium">Contraseña</label>
        <div class="relative flex items-center">
          <input
            id="password"
            :value="props.password"
            :type="passwordInputType"
            placeholder="••••••••"
            class="w-full h-12 rounded-lg bg-[#0F0F1A] border border-[#393960] text-white placeholder-[#9898c3] pl-4 pr-12 focus:outline-none focus:border-[#191971] focus:ring-1 focus:ring-[#191971] transition-all duration-200"
            @input="onPasswordInput"
            required
          />
          <button
            type="button"
            class="absolute right-3 text-[#9898c3] hover:text-white transition-colors flex items-center justify-center p-1"
            @click="showPassword = !showPassword"
          >
            <span class="material-symbols-outlined text-[20px]">{{ passwordIcon }}</span>
          </button>
        </div>
      </div>

      <div class="flex items-center justify-between mt-1">
        <label class="flex items-center gap-2 cursor-pointer group/check">
          <div class="relative flex items-center">
            <input
              type="checkbox"
              class="peer h-4 w-4 appearance-none rounded border border-[#393960] bg-[#0F0F1A] checked:border-[#191971] checked:bg-[#191971] focus:ring-0 focus:ring-offset-0 transition-colors"
            />
            <span
              class="material-symbols-outlined absolute text-white opacity-0 peer-checked:opacity-100 pointer-events-none text-sm top-0 left-0"
            >
              check
            </span>
          </div>
          <span class="text-sm text-[#9898c3] group-hover/check:text-white transition-colors">Recordarme</span>
        </label>
        <router-link
          to="/auth/forgot-password"
          class="text-sm text-[#85B2B2] hover:text-[#A5D2D2] transition-colors font-medium"
        >
          ¿Olvidaste tu contraseña?
        </router-link>
      </div>

      <AppButton type="submit" :loading="loading" icon="login" class="mt-4">
        {{ loading ? 'Iniciando sesión...' : 'Iniciar sesión' }}
      </AppButton>
    </form>

    <div class="mt-8 text-center">
      <p class="text-sm text-[#9898c3]">
        ¿No tienes una cuenta?
        <router-link to="/auth/register" class="text-[#85B2B2] hover:text-[#A5D2D2] font-medium transition-colors">
          Regístrate
        </router-link>
      </p>
    </div>
  </SurfaceCard>
</template>

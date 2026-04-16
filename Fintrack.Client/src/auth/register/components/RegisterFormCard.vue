<script setup lang="ts">
import { computed, ref } from 'vue';
import AppButton from '@/app/components/common/AppButton.vue';
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';

type PasswordStrength = {
  label: string;
  widthClass: string;
  colorClass: string;
};

const props = defineProps<{
  fullName: string;
  email: string;
  password: string;
  terms: boolean;
  loading: boolean;
  errorMsg: string;
  passwordStrength: PasswordStrength;
}>();

const emit = defineEmits<{
  'update:fullName': [value: string];
  'update:email': [value: string];
  'update:password': [value: string];
  'update:terms': [value: boolean];
  submit: [];
}>();

const showPassword = ref(false);
const passwordInputType = computed(() => (showPassword.value ? 'text' : 'password'));
const passwordIcon = computed(() => (showPassword.value ? 'visibility_off' : 'visibility'));

const onFullNameInput = (event: Event) => {
  emit('update:fullName', (event.target as HTMLInputElement).value);
};

const onEmailInput = (event: Event) => {
  emit('update:email', (event.target as HTMLInputElement).value);
};

const onPasswordInput = (event: Event) => {
  emit('update:password', (event.target as HTMLInputElement).value);
};

const onTermsChange = (event: Event) => {
  emit('update:terms', (event.target as HTMLInputElement).checked);
};
</script>

<template>
  <SurfaceCard class="w-full max-w-md rounded-md bg-[#1A1A30] border border-white/5 layered-obsidian-shadow !p-8">
    <div class="mb-8">
      <h1 class="text-2xl font-bold tracking-tight text-white">Crear Cuenta</h1>
      <p class="mt-2 text-sm text-[#AAAAAA]">Protege tus activos digitales con seguridad de nivel empresarial.</p>
      <p v-if="errorMsg" class="mt-2 text-red-500 text-sm font-semibold">{{ errorMsg }}</p>
    </div>

    <form class="space-y-5" @submit.prevent="emit('submit')">
      <div class="space-y-2">
        <label for="full-name" class="text-sm font-semibold tracking-wide text-white">Nombre Completo</label>
        <input
          id="full-name"
          :value="props.fullName"
          type="text"
          placeholder="Ingresa tu nombre completo"
          class="w-full rounded-md border border-white/10 bg-[#0F0F1A]/50 px-4 py-3 text-white placeholder:text-[#AAAAAA]/50 focus:border-[#191971] focus:ring-1 focus:ring-[#191971] outline-none transition-all input-focus-inset font-display"
          required
          @input="onFullNameInput"
        />
      </div>

      <div class="space-y-2">
        <label for="email" class="text-sm font-semibold tracking-wide text-white">Correo Electronico</label>
        <input
          id="email"
          :value="props.email"
          type="email"
          placeholder="nombre@ejemplo.com"
          class="w-full rounded-md border border-white/10 bg-[#0F0F1A]/50 px-4 py-3 text-white placeholder:text-[#AAAAAA]/50 focus:border-[#191971] focus:ring-1 focus:ring-[#191971] outline-none transition-all input-focus-inset font-display"
          required
          @input="onEmailInput"
        />
      </div>

      <div class="space-y-2">
        <label for="password" class="text-sm font-semibold tracking-wide text-white">Contrasena</label>
        <div class="relative">
          <input
            id="password"
            :value="props.password"
            :type="passwordInputType"
            placeholder="••••••••"
            class="w-full rounded-md border border-white/10 bg-[#0F0F1A]/50 px-4 py-3 pr-12 text-white placeholder:text-[#AAAAAA]/50 focus:border-[#191971] focus:ring-1 focus:ring-[#191971] outline-none transition-all input-focus-inset font-display"
            required
            @input="onPasswordInput"
          />
          <button
            type="button"
            class="absolute right-3 top-1/2 -translate-y-1/2 text-[#AAAAAA] hover:text-white"
            @click="showPassword = !showPassword"
          >
            <span class="material-symbols-outlined text-xl">{{ passwordIcon }}</span>
          </button>
        </div>
        <div class="mt-2 space-y-1.5">
          <div class="w-full bg-white/10 h-1 flex rounded-full overflow-hidden">
            <div :class="[passwordStrength.widthClass, passwordStrength.colorClass]" class="h-full transition-all duration-300" />
          </div>
          <p class="text-[10px] font-bold uppercase tracking-widest text-[#85B2B2]">Seguridad: {{ passwordStrength.label }}</p>
        </div>
      </div>

      <div class="flex items-start gap-3 py-2">
        <div class="flex h-5 items-center">
          <input
            id="terms"
            name="terms"
            :checked="props.terms"
            type="checkbox"
            class="h-4 w-4 rounded border-white/20 bg-[#0F0F1A] text-[#191971] focus:ring-[#191971] focus:ring-offset-[#0F0F1A]"
            @change="onTermsChange"
          />
        </div>
        <label for="terms" class="text-xs leading-relaxed text-[#AAAAAA]">
          Acepto los <a href="#" class="text-[#85B2B2] hover:underline">Terminos de Servicio</a>
          y la <a href="#" class="text-[#85B2B2] hover:underline">Politica de Privacidad</a>.
        </label>
      </div>

      <AppButton type="submit" :loading="loading" class="w-full !py-4 tracking-[0.05em]">
        {{ loading ? 'Creando cuenta...' : 'Crear cuenta' }}
      </AppButton>
    </form>

    <p class="mt-8 text-center text-sm font-medium text-[#AAAAAA]">
      Ya tienes una cuenta?
      <router-link to="/auth/login" class="font-bold text-[#85B2B2] hover:underline ml-1">Inicia sesion</router-link>
    </p>
  </SurfaceCard>
</template>

<style scoped>
.layered-obsidian-shadow {
  box-shadow: 0 10px 30px -5px rgba(25, 25, 113, 0.2);
}

.input-focus-inset:focus {
  box-shadow: inset 0 2px 4px 0 rgba(0, 0, 0, 0.06);
}
</style>

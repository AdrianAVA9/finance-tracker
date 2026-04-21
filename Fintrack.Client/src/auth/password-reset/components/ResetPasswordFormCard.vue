<script setup lang="ts">
import { computed, ref } from 'vue';
import AppButton from '@/app/components/common/AppButton.vue';
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';

const props = defineProps<{
  newPassword: string;
  confirmPassword: string;
  isLoading: boolean;
  error: string;
  canSubmit: boolean;
  validationError: string;
}>();

const emit = defineEmits<{
  (e: 'update:newPassword', value: string): void;
  (e: 'update:confirmPassword', value: string): void;
  (e: 'submit'): void;
}>();

const showPassword = ref(false);
const showConfirmPassword = ref(false);

const passwordStrength = computed(() => {
  const pw = props.newPassword;
  if (pw.length === 0) return { label: 'Ninguna', width: '0%', color: '#1e293b' };
  if (pw.length < 6) return { label: 'Débil', width: '25%', color: '#ef4444' };
  if (pw.length < 10) return { label: 'Media', width: '55%', color: '#f59e0b' };
  if (pw.match(/[A-Z]/) && pw.match(/[0-9]/) && pw.match(/[^a-zA-Z0-9]/)) {
    return { label: 'Fuerte', width: '100%', color: '#1e6a7b' };
  }

  return { label: 'Buena', width: '75%', color: '#22c55e' };
});

const confirmInputClass = computed(() => [
  'block w-full rounded-xl border bg-slate-800 py-3.5 px-4 pr-12 text-white outline-none transition-all placeholder:text-slate-600 focus:ring-1',
  props.confirmPassword && props.validationError
    ? 'border-red-500/70 focus:border-red-500 focus:ring-red-500/50'
    : 'border-slate-700 focus:border-[#1e6a7b] focus:ring-[#1e6a7b]',
]);
</script>

<template>
  <SurfaceCard>
    <p v-if="error" class="mb-5 rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-center text-sm font-medium text-red-400">
      {{ error }}
    </p>

    <form class="space-y-6" @submit.prevent="emit('submit')">
      <div class="space-y-2">
        <label class="ml-1 text-sm font-semibold text-slate-300" for="new-password">Nueva Contraseña</label>
        <div class="relative flex items-center">
          <input
            id="new-password"
            :value="newPassword"
            :type="showPassword ? 'text' : 'password'"
            placeholder="Ingresa al menos 6 caracteres"
            required
            class="block w-full rounded-xl border border-slate-700 bg-slate-800 py-3.5 px-4 pr-12 text-white outline-none transition-all placeholder:text-slate-600 focus:border-[#1e6a7b] focus:ring-1 focus:ring-[#1e6a7b]"
            @input="emit('update:newPassword', ($event.target as HTMLInputElement).value)"
          />
          <button
            type="button"
            class="absolute right-4 text-slate-400 transition-colors hover:text-[#1e6a7b]"
            @click="showPassword = !showPassword"
          >
            <span class="material-symbols-outlined text-xl">{{ showPassword ? 'visibility_off' : 'visibility' }}</span>
          </button>
        </div>
      </div>

      <div class="space-y-3">
        <div class="flex items-center justify-between px-1">
          <span class="text-xs font-medium uppercase tracking-wider text-slate-500">Seguridad de la Contraseña</span>
          <span class="text-xs font-bold" :style="{ color: passwordStrength.color }">{{ passwordStrength.label }}</span>
        </div>
        <div class="h-1.5 w-full overflow-hidden rounded-full bg-slate-800">
          <div class="h-full rounded-full transition-all duration-500" :style="{ width: passwordStrength.width, backgroundColor: passwordStrength.color }" />
        </div>
        <p class="text-[11px] italic text-slate-500">Consejo: Usa una mezcla de mayúsculas, minúsculas, números y símbolos.</p>
      </div>

      <div class="space-y-2">
        <label class="ml-1 text-sm font-semibold text-slate-300" for="confirm-password">Confirmar Contraseña</label>
        <div class="relative flex items-center">
          <input
            id="confirm-password"
            :value="confirmPassword"
            :type="showConfirmPassword ? 'text' : 'password'"
            placeholder="Repite la nueva contraseña"
            required
            :class="confirmInputClass"
            @input="emit('update:confirmPassword', ($event.target as HTMLInputElement).value)"
          />
          <button
            type="button"
            class="absolute right-4 text-slate-400 transition-colors hover:text-[#1e6a7b]"
            @click="showConfirmPassword = !showConfirmPassword"
          >
            <span class="material-symbols-outlined text-xl">{{ showConfirmPassword ? 'visibility_off' : 'visibility' }}</span>
          </button>
        </div>
        <p v-if="validationError" class="ml-1 text-xs text-red-400">{{ validationError }}</p>
      </div>

      <AppButton type="submit" :disabled="isLoading || !canSubmit" :loading="isLoading" icon="verified_user">
        {{ isLoading ? 'Actualizando...' : 'Actualizar Contraseña' }}
      </AppButton>
    </form>
  </SurfaceCard>
</template>

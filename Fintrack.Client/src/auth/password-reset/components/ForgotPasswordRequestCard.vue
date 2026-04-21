<script setup lang="ts">
import AppButton from '@/app/components/common/AppButton.vue';
import SurfaceCard from '@/app/components/common/SurfaceCard.vue';

defineProps<{
  email: string;
  isLoading: boolean;
  error: string;
}>();

const emit = defineEmits<{
  (e: 'update:email', value: string): void;
  (e: 'submit'): void;
}>();
</script>

<template>
  <SurfaceCard>
    <p v-if="error" class="mb-4 rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-center text-sm font-medium text-red-400">
      {{ error }}
    </p>

    <form class="space-y-6" @submit.prevent="emit('submit')">
      <div class="space-y-2">
        <label class="ml-1 block text-sm font-medium text-slate-200" for="reset-email">
          Correo Electrónico
        </label>
        <div class="relative">
          <span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-xl text-slate-500">mail</span>
          <input
            id="reset-email"
            :value="email"
            type="email"
            placeholder="name@company.com"
            required
            class="w-full rounded-lg border border-slate-800 bg-slate-950 py-3.5 pl-12 pr-4 text-slate-100 placeholder:text-slate-600 outline-none transition-all focus:border-[#1e6a7b] focus:ring-2 focus:ring-[#1e6a7b]/50"
            @input="emit('update:email', ($event.target as HTMLInputElement).value)"
          />
        </div>
      </div>

      <AppButton type="submit" :loading="isLoading" icon="arrow_forward">
        {{ isLoading ? 'Enviando...' : 'Enviar Enlace' }}
      </AppButton>
    </form>
  </SurfaceCard>
</template>

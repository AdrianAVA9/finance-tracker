<script setup lang="ts">
import SurfaceCard from './SurfaceCard.vue'

const amount = defineModel<number | null>({ required: true })

withDefaults(
  defineProps<{
    currencySymbol?: string
    placeholder?: string
    step?: string
    required?: boolean
    inputId?: string
  }>(),
  {
    currencySymbol: '₡',
    placeholder: '0',
    step: '0.01',
    required: true,
  },
)
</script>

<template>
  <SurfaceCard
    class="!border-white/[0.03] !p-10 flex flex-col items-center justify-center transition-all duration-500 focus-within:border-primary-container/20 focus-within:bg-primary-container/[0.02] luminous-shadow-sm"
  >
    <div class="flex items-baseline gap-2">
      <span class="font-headline text-4xl font-black text-primary-container">{{ currencySymbol }}</span>
      <input
        :id="inputId"
        v-model.number="amount"
        type="number"
        class="amount-input bg-transparent border-none text-center font-headline text-6xl font-black tracking-tighter text-on-surface focus:ring-0 w-full max-w-[280px] placeholder:text-on-surface-variant/20"
        :placeholder="placeholder"
        :step="step"
        :required="required"
      >
    </div>
  </SurfaceCard>
</template>

<style scoped>
.luminous-shadow-sm {
  box-shadow: 0 20px 40px -20px rgba(5, 230, 153, 0.05);
}

.amount-input::-webkit-outer-spin-button,
.amount-input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.amount-input[type='number'] {
  -moz-appearance: textfield;
  appearance: textfield;
}
</style>

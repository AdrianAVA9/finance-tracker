<script setup lang="ts">
import MonthPicker from '@/app/components/common/MonthPicker.vue'

defineProps<{
  selectedMonth: number
  selectedYear: number
  selectedType: 'All' | 'Income' | 'Expense'
}>()

const emit = defineEmits<{
  (event: 'update:selectedMonth', value: number): void
  (event: 'update:selectedYear', value: number): void
  (event: 'select-type', value: 'All' | 'Income' | 'Expense'): void
}>()

const filterOptions: Array<{ label: string; value: 'All' | 'Income' | 'Expense' }> = [
  { label: 'Todo', value: 'All' },
  { label: 'Ingresos', value: 'Income' },
  { label: 'Gastos', value: 'Expense' },
]
</script>

<template>
  <section class="space-y-6">
    <div class="flex items-center justify-between px-1">
      <MonthPicker
        :month="selectedMonth"
        :year="selectedYear"
        @update:month="emit('update:selectedMonth', $event)"
        @update:year="emit('update:selectedYear', $event)"
      />
      <div class="font-label text-[10px] font-bold uppercase tracking-widest text-on-surface-variant/40">
        Resumen Mensual
      </div>
    </div>

    <div class="no-scrollbar flex gap-2 overflow-x-auto py-2">
      <button
        v-for="option in filterOptions"
        :key="option.value"
        class="whitespace-nowrap rounded-xl px-6 py-2.5 text-xs font-black uppercase tracking-widest transition-all duration-300"
        :class="
          selectedType === option.value
            ? 'bg-primary-container text-on-primary shadow-lg shadow-primary-container/20'
            : 'bg-surface-container-high text-on-surface-variant hover:bg-surface-variant'
        "
        @click="emit('select-type', option.value)"
      >
        {{ option.label }}
      </button>
    </div>
  </section>
</template>

<style scoped>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}

.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>

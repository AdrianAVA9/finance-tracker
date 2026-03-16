<script setup lang="ts">
import { computed, ref } from 'vue'

interface Category {
  id: number
  name: string
  budgetAmount: number
}

interface Props {
  groupName: string
  categories: Category[]
  totalIncome: number
  threshold?: number // Percentage threshold for warning (e.g. 55)
}

const props = defineProps<Props>()
const emit = defineEmits(['update:categoryAmount'])

const isOpen = ref(true)

const groupTotal = computed(() => {
  return props.categories.reduce((sum, cat) => sum + (Number(cat.budgetAmount) || 0), 0)
})

const percentageOfIncome = computed(() => {
  if (!props.totalIncome || props.totalIncome <= 0) return 0
  return (groupTotal.value / props.totalIncome) * 100
})

const isOverThreshold = computed(() => {
  if (!props.threshold) return false
  return percentageOfIncome.value > props.threshold
})

const handleInput = (categoryId: number, value: string) => {
  emit('update:categoryAmount', { categoryId, amount: Number(value) || 0 })
}
</script>

<template>
  <div class="border border-border-dark rounded-xl bg-surface-dark overflow-hidden transition-all duration-300">
    <!-- Header -->
    <div 
      @click="isOpen = !isOpen"
      class="px-6 py-4 flex items-center justify-between cursor-pointer hover:bg-white/5 transition-colors"
    >
      <div class="flex items-center gap-3">
        <span class="material-symbols-outlined text-primary transition-transform duration-300" :class="{ 'rotate-180': isOpen }">
          expand_more
        </span>
        <div class="flex flex-col">
          <h3 class="font-bold text-text-main text-lg">{{ groupName }}</h3>
          <div class="flex items-center gap-2 mt-0.5">
            <div class="w-32 h-1.5 bg-background-dark rounded-full overflow-hidden">
              <div 
                class="h-full transition-all duration-500"
                :class="isOverThreshold ? 'bg-red-500 shadow-[0_0_8px_rgba(239,68,68,0.5)]' : 'bg-primary'"
                :style="{ width: `${Math.min(percentageOfIncome, 100)}%` }"
              ></div>
            </div>
            <span class="text-[10px] uppercase tracking-widest font-bold" :class="isOverThreshold ? 'text-red-400' : 'text-text-muted'">
              {{ percentageOfIncome.toFixed(1) }}%
            </span>
          </div>
        </div>
      </div>
      
      <div class="text-right">
        <span class="text-xs text-text-muted uppercase tracking-tighter">Total Presupuestado</span>
        <p class="text-xl font-black text-text-main">
          ₡{{ groupTotal.toLocaleString('es-CR', { minimumFractionDigits: 2 }) }}
        </p>
      </div>
    </div>

    <!-- Content -->
    <div v-show="isOpen" class="px-6 pb-6 pt-2 border-t border-border-dark/50">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-4">
        <div v-for="cat in categories" :key="cat.id" class="flex flex-col gap-1.5">
          <label class="text-xs font-semibold text-text-muted ml-1">{{ cat.name }}</label>
          <div class="relative group">
            <span class="absolute left-3 top-1/2 -translate-y-1/2 text-primary font-bold text-sm">₡</span>
            <input 
              :value="cat.budgetAmount || ''"
              @input="e => handleInput(cat.id, (e.target as HTMLInputElement).value)"
              type="number" 
              step="0.01"
              class="w-full bg-surface-input border border-border-dark rounded-lg py-2.5 pl-8 pr-4 text-text-main focus:ring-1 focus:ring-primary focus:border-primary outline-none transition-all placeholder:text-text-muted/30 font-medium"
              placeholder="0.00"
            />
          </div>
        </div>
      </div>
      
      <!-- Warning Message -->
      <div v-if="isOverThreshold" class="mt-4 p-3 bg-red-500/10 border border-red-500/20 rounded-lg flex items-center gap-2">
        <span class="material-symbols-outlined text-red-400 text-sm">warning</span>
        <span class="text-[11px] text-red-400 font-medium">Esta sección excede el {{ threshold }}% recomendado de tus ingresos.</span>
      </div>
    </div>
  </div>
</template>

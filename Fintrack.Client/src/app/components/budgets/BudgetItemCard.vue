<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'

interface Budget {
  id: string
  categoryId: string
  categoryName: string
  categoryIcon?: string
  categoryColor?: string
  categoryGroup?: string
  limitAmount: number
  spentAmount: number
  isRecurrent?: boolean
}

const props = defineProps<{
  budget: Budget
}>()

const emit = defineEmits<{
  (e: 'edit', budget: Budget): void
  (e: 'delete', id: string): void
}>()

const isMenuOpen = ref(false)
const menuRef = ref<HTMLElement | null>(null)

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value
}

const handleEdit = () => {
  isMenuOpen.value = false
  emit('edit', props.budget)
}

const handleDelete = () => {
  isMenuOpen.value = false
  emit('delete', props.budget.id)
}

const closeMenu = (e: MouseEvent) => {
  if (menuRef.value && !menuRef.value.contains(e.target as Node)) {
    isMenuOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', closeMenu)
})

onUnmounted(() => {
  document.removeEventListener('click', closeMenu)
})

const spentRatio = computed(() => {
  if (props.budget.limitAmount === 0) return 0
  return props.budget.spentAmount / props.budget.limitAmount
})

/** Negative only when gasto real supera el límite (no cuando disponible === 0). */
const remainingAmount = computed(() => props.budget.limitAmount - props.budget.spentAmount)
const isOverBudget = computed(() => remainingAmount.value < 0)

const progressColorClass = computed(() => {
  if (isOverBudget.value) return 'bg-red-500'
  return 'bg-emerald-500'
})

const statusLabel = computed(() => {
  const diff = remainingAmount.value
  if (diff >= 0) {
    return `Disponible ₡${formatCurrency(diff)}`
  }
  return `Excedido ₡${formatCurrency(Math.abs(diff))}`
})

const statusLabelColorClass = computed(() => {
  if (isOverBudget.value) return 'text-red-500 font-black'
  return 'text-[#05E699] font-bold'
})

const infoText = computed(() => {
  return `₡${formatCurrency(props.budget.spentAmount)} / ₡${formatCurrency(props.budget.limitAmount)}`
})

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('es-CR', { minimumFractionDigits: 0, maximumFractionDigits: 0 })
}
</script>

<template>
  <div 
    @click="handleEdit"
    class="group relative p-6 rounded-2xl bg-surface-container-low hover:bg-surface-container transition-all duration-300 transform hover:-translate-y-1 overflow-visible cursor-pointer border border-white/[0.03] luminous-shadow-sm"
    :class="{ 'z-[100]': isMenuOpen }"
  >
    <!-- Header: Icon, Info, and Menu -->
    <div class="flex items-start gap-4 mb-6">
      <!-- Category Icon -->
      <div class="w-12 h-12 rounded-md bg-surface-container-highest flex items-center justify-center text-primary grow-0 shrink-0 border border-white/5 shadow-inner relative">
        <span class="material-symbols-outlined text-2xl" :style="{ color: isOverBudget ? '#ef4444' : '#05E699' }">
          {{ budget.categoryIcon || 'category' }}
        </span>

        <!-- Recurring Badge (The Status Badge) -->
        <div 
          v-if="budget.isRecurrent" 
          class="absolute -bottom-1 -right-1 w-5 h-5 rounded-full bg-primary flex items-center justify-center border-2 border-[#1a1c20] shadow-lg z-10"
          title="Presupuesto Recurrente"
        >
          <span class="material-symbols-outlined text-[11px] text-[#111317] font-black" style="font-variation-settings: 'FILL' 1;">sync</span>
        </div>
      </div>
      
      <!-- Info Column -->
      <div class="flex-1 min-w-0 pt-0.5">
        <div class="flex justify-between items-start">
          <div class="flex-1 min-w-0 space-y-1">
            <h4 class="font-headline font-semibold text-on-surface truncate leading-tight tracking-tight">{{ budget.categoryName }}</h4>
            
            <p class="text-[10px] text-on-surface-variant font-medium tracking-wide">{{ budget.categoryGroup || 'Categoría' }}</p>
          </div>

          <!-- Menu Button -->
          <div class="relative ml-2" ref="menuRef">
            <button 
              @click.stop="toggleMenu"
              class="p-2 rounded-xl hover:bg-surface-container-highest text-on-surface-variant transition-colors"
              aria-label="Más opciones"
            >
              <span class="material-symbols-outlined text-2xl">more_vert</span>
            </button>

            <!-- Dropdown Menu -->
            <transition 
              enter-active-class="transition duration-200 ease-out" 
              enter-from-class="transform scale-90 opacity-0" 
              enter-to-class="transform scale-100 opacity-100" 
              leave-active-class="transition duration-150 ease-in" 
              leave-from-class="transform scale-100 opacity-100" 
              leave-to-class="transform scale-90 opacity-0"
            >
              <div 
                v-if="isMenuOpen" 
                class="absolute right-0 top-full mt-2 w-44 bg-surface-container-highest border border-white/[0.05] rounded-2xl shadow-2xl z-[60] py-2 overflow-hidden backdrop-blur-xl"
              >
                <button @click="handleEdit" class="w-full text-left px-5 py-3 text-[13px] font-bold text-on-surface hover:bg-white/5 flex items-center gap-3 transition-colors">
                  <span class="material-symbols-outlined text-xl">edit</span> Editar Presupuesto
                </button>
                <button @click="handleDelete" class="w-full text-left px-5 py-3 text-[13px] font-bold text-red-500 hover:bg-red-500/10 flex items-center gap-3 border-t border-white/5 transition-colors">
                  <span class="material-symbols-outlined text-xl">delete</span> Eliminar
                </button>
              </div>
            </transition>
          </div>
        </div>
      </div>
    </div>

    <!-- Progress Information -->
    <div class="space-y-2">
      <div class="flex justify-between text-[11px] font-bold leading-none px-0.5">
        <span :class="statusLabelColorClass">{{ statusLabel }}</span>
        <span class="text-on-surface-variant/70">{{ infoText }}</span>
      </div>
      
      <div class="w-full h-1 bg-surface-container-highest rounded-full overflow-hidden shadow-inner">
        <div 
          class="h-full rounded-full transition-all duration-1000 ease-out luminous-shadow-sm"
          :class="progressColorClass"
          :style="{ width: `${Math.min(spentRatio * 100, 100)}%` }"
        ></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>

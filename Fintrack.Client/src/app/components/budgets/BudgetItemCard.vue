<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'

interface Budget {
  id: number
  categoryId: number
  categoryName: string
  categoryIcon?: string
  categoryColor?: string
  categoryGroup?: string
  limitAmount: number
  spentAmount: number
}

const props = defineProps<{
  budget: Budget
}>()

const emit = defineEmits<{
  (e: 'edit', budget: Budget): void
  (e: 'delete', id: number): void
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

const percentage = computed(() => Math.round(spentRatio.value * 100))

const progressColorClass = computed(() => {
  if (spentRatio.value >= 1) return 'bg-red-500 shadow-[0_0_8px_rgba(239,68,68,0.4)]'
  if (spentRatio.value >= 0.8) return 'bg-orange-500 shadow-[0_0_8px_rgba(249,115,22,0.3)]'
  return 'bg-primary shadow-[0_0_8px_rgba(31,107,122,0.3)]'
})

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('es-CR', { minimumFractionDigits: 0, maximumFractionDigits: 0 })
}
</script>

<template>
  <div 
    class="relative p-4 rounded-2xl bg-surface-light dark:bg-[#1e293b]/40 border border-slate-200 dark:border-slate-800/50 hover:border-primary/30 transition-all duration-300 shadow-sm"
  >
    <!-- Header: Icon, Name, and Menu -->
    <div class="flex items-center justify-between gap-3">
      <div class="flex items-center gap-3 min-w-0">
        <div class="w-10 h-10 rounded-xl bg-slate-100 dark:bg-slate-800 flex items-center justify-center text-primary grow-0 shrink-0">
          <span class="material-symbols-outlined text-xl">{{ budget.categoryIcon || 'category' }}</span>
        </div>
        <div class="flex flex-col min-w-0">
          <h3 class="text-sm font-bold text-slate-900 dark:text-white truncate">{{ budget.categoryName }}</h3>
          <p class="text-[9px] uppercase tracking-widest font-extrabold text-slate-500 dark:text-slate-500 truncate mt-0.5">
            {{ budget.categoryGroup }}
          </p>
        </div>
      </div>

      <!-- Compact Menu Button -->
      <div class="relative" ref="menuRef">
        <button 
          @click.stop="toggleMenu"
          class="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-400 transition-colors"
          aria-label="Más opciones"
        >
          <span class="material-symbols-outlined text-xl">more_vert</span>
        </button>

        <!-- Dropdown Menu -->
        <transition 
          enter-active-class="transition duration-100 ease-out" 
          enter-from-class="transform scale-95 opacity-0" 
          enter-to-class="transform scale-100 opacity-100" 
          leave-active-class="transition duration-75 ease-in" 
          leave-from-class="transform scale-100 opacity-100" 
          leave-to-class="transform scale-95 opacity-0"
        >
          <div 
            v-if="isMenuOpen" 
            class="absolute right-0 top-full mt-1 w-32 bg-white dark:bg-[#0f172a] border border-slate-200 dark:border-slate-800 rounded-xl shadow-xl z-30 py-1 overflow-hidden"
          >
            <button @click="handleEdit" class="w-full text-left px-4 py-2 text-[13px] font-medium text-slate-600 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-800 flex items-center gap-2">
              <span class="material-symbols-outlined text-lg">edit</span> Editar
            </button>
            <button @click="handleDelete" class="w-full text-left px-4 py-2 text-[13px] font-medium text-red-500 hover:bg-red-50 dark:hover:bg-red-500/10 flex items-center gap-2 border-t border-slate-100 dark:border-slate-800/50">
              <span class="material-symbols-outlined text-lg">delete</span> Eliminar
            </button>
          </div>
        </transition>
      </div>
    </div>

    <!-- Progress Information -->
    <div class="mt-4 space-y-2">
      <div class="flex justify-between items-end">
        <div class="flex items-baseline gap-1.5">
          <span class="text-base font-black text-slate-900 dark:text-white">₡{{ formatCurrency(budget.spentAmount) }}</span>
          <span class="text-[10px] text-slate-400 font-bold">/ ₡{{ formatCurrency(budget.limitAmount) }}</span>
        </div>
        <span class="text-[11px] font-black" :class="spentRatio >= 1 ? 'text-red-500' : 'text-primary'">
          {{ percentage }}%
        </span>
      </div>
      
      <div class="w-full h-1.5 bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden">
        <div 
          class="h-full rounded-full transition-all duration-700 ease-out"
          :class="progressColorClass"
          :style="{ width: `${Math.min(spentRatio * 100, 100)}%` }"
        ></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 1, 'wght' 400, 'GRAD' 0, 'opsz' 20;
}
</style>

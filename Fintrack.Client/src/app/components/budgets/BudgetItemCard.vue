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

const progressColorClass = computed(() => {
  if (spentRatio.value >= 1) return 'bg-red-500'
  return 'bg-primary-container'
})

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('es-CR', { minimumFractionDigits: 0, maximumFractionDigits: 0 })
}
</script>

<template>
  <div 
    class="group relative p-5 rounded-lg bg-[#1a1c20] hover:bg-[#222428] transition-all duration-300 transform hover:-translate-y-1 overflow-visible"
    :class="{ 'z-[100]': isMenuOpen }"
  >
    <!-- Header: Icon, Name, and Menu -->
    <div class="flex items-center gap-4 mb-5">
      <div class="w-12 h-12 rounded-md bg-surface-container-highest flex items-center justify-center text-primary-container grow-0 shrink-0">
        <span class="material-symbols-outlined text-2xl" :style="{ color: spentRatio >= 1 ? '#FF4D4D' : '#05E699' }">
          {{ budget.categoryIcon || 'category' }}
        </span>
      </div>
      
      <div class="flex-1 min-w-0">
        <div class="flex justify-between items-start">
          <h4 class="font-headline font-semibold text-on-surface truncate">{{ budget.categoryName }}</h4>
          <span class="text-sm font-bold text-on-surface whitespace-nowrap ml-2">₡{{ formatCurrency(budget.limitAmount) }}</span>
        </div>
        <p class="text-[10px] text-on-surface-variant font-medium tracking-wide uppercase">{{ budget.categoryGroup || 'Categoría' }}</p>
      </div>

      <!-- Menu Button -->
      <div class="relative" ref="menuRef">
        <button 
          @click.stop="toggleMenu"
          class="p-1.5 rounded-lg hover:bg-surface-container-highest text-on-surface-variant transition-colors"
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
            class="absolute right-0 top-full mt-2 w-36 bg-surface-container-highest border border-white/[0.03] rounded-lg shadow-2xl z-[60] py-1 overflow-hidden"
          >
            <button @click="handleEdit" class="w-full text-left px-4 py-2.5 text-[12px] font-bold text-on-surface hover:bg-white/5 flex items-center gap-2">
              <span class="material-symbols-outlined text-lg">edit</span> Editar
            </button>
            <button @click="handleDelete" class="w-full text-left px-4 py-2.5 text-[12px] font-bold text-red-500 hover:bg-red-500/10 flex items-center gap-2 border-t border-white/5">
              <span class="material-symbols-outlined text-lg">delete</span> Eliminar
            </button>
          </div>
        </transition>
      </div>
    </div>

    <!-- Progress Information -->
    <div class="space-y-2">
      <div class="flex justify-between text-[10px] font-bold text-on-surface-variant uppercase tracking-widest leading-none">
        <span>Progreso</span>
        <span class="text-on-surface">₡{{ formatCurrency(budget.spentAmount) }} / ₡{{ formatCurrency(budget.limitAmount) }}</span>
      </div>
      
      <div class="w-full h-1 bg-surface-container-highest rounded-full overflow-hidden">
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
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>

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
  isRecurrent?: boolean
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
    @click="handleEdit"
    class="group relative p-6 rounded-2xl bg-surface-container-low hover:bg-surface-container transition-all duration-300 transform hover:-translate-y-1 overflow-visible cursor-pointer border border-white/[0.03] luminous-shadow-sm"
    :class="{ 'z-[100]': isMenuOpen }"
  >
    <!-- Header: Icon, Info, and Menu -->
    <div class="flex items-start gap-4 mb-6">
      <!-- Category Icon -->
      <div class="w-14 h-14 rounded-2xl bg-surface-container-highest flex items-center justify-center text-primary grow-0 shrink-0 border border-white/5 shadow-inner">
        <span class="material-symbols-outlined text-3xl" :style="{ color: spentRatio >= 1 ? '#ef4444' : '#05E699' }">
          {{ budget.categoryIcon || 'category' }}
        </span>
      </div>
      
      <!-- Info Column -->
      <div class="flex-1 min-w-0 pt-0.5">
        <div class="flex justify-between items-start">
          <div class="flex-1 min-w-0 space-y-1">
            <h4 class="font-headline font-bold text-2xl text-on-surface truncate leading-tight tracking-tight">{{ budget.categoryName }}</h4>
            
            <p class="text-[13px] font-semibold text-on-surface-variant tracking-normal">
              ₡{{ formatCurrency(Math.max(budget.limitAmount - budget.spentAmount, 0)) }} restantes de ₡{{ formatCurrency(budget.limitAmount) }}
            </p>

            <!-- Recurring Label (Visible only if true) -->
            <div v-if="budget.isRecurrent" class="flex items-center gap-1.5 mt-2 py-0.5 text-primary">
              <span class="material-symbols-outlined text-[18px]" style="font-variation-settings: 'FILL' 1;">sync</span>
              <span class="text-[10px] font-black uppercase tracking-[0.2em] pt-0.5">Recurrente</span>
            </div>
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
    <div class="space-y-4">
      <div class="w-full h-1.5 bg-surface-container-highest rounded-full overflow-hidden shadow-inner">
        <div 
          class="h-full rounded-full transition-all duration-1000 ease-out luminous-shadow-sm"
          :class="progressColorClass"
          :style="{ width: `${Math.min(spentRatio * 100, 100)}%` }"
        ></div>
      </div>
      
      <div class="flex justify-between items-center text-[10px] font-black uppercase tracking-[0.2em] leading-none px-0.5">
        <span class="text-on-surface-variant/80">{{ Math.round(spentRatio * 100) }}% Consumido</span>
        <span 
          class="font-black px-2 py-0.5 rounded-full"
          :class="spentRatio >= 1 ? 'text-red-500' : 'text-primary'"
        >
          {{ spentRatio >= 1 ? 'Excedido' : 'En Orden' }}
        </span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>

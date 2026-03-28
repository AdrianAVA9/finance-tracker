<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import api from '@/services/api'
import CategoryBudgetModal from '@/app/components/budgets/CategoryBudgetModal.vue'
import BudgetItemCard from '@/app/components/budgets/BudgetItemCard.vue'

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

const currentDate = new Date()
const selectedMonth = ref(currentDate.getMonth() + 1)
const selectedYear = ref(currentDate.getFullYear())
const expectedIncome = ref<number>(0)
const budgets = ref<Budget[]>([])
const isLoading = ref(false)
const searchQuery = ref('')
const sortBy = ref('Highest Budget')

// Modal State
const isModalOpen = ref(false)
const selectedBudget = ref<Budget | null>(null)

// Constants
const months = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
]
const years = [2024, 2025, 2026, 2027]

// Computed Stats
const totalBudgeted = computed(() => budgets.value.reduce((sum, b) => sum + b.limitAmount, 0))
const totalSpent = computed(() => budgets.value.reduce((sum, b) => sum + b.spentAmount, 0))
const remainingToAllocate = computed(() => expectedIncome.value - totalBudgeted.value)
const spentPercentage = computed(() => totalBudgeted.value > 0 ? (totalSpent.value / totalBudgeted.value) * 100 : 0)

// Dynamic UI for Remaining Balance
const isOverbudgeted = computed(() => remainingToAllocate.value < 0)
const remainingColorClass = computed(() => isOverbudgeted.value ? 'text-red-500' : 'text-accent-lime')
const remainingStatusLabel = computed(() => isOverbudgeted.value ? 'Excedido' : 'En Orden')
const remainingStatusClass = computed(() => isOverbudgeted.value ? 'bg-red-500/10 text-red-500' : 'bg-accent-lime/10 text-accent-lime')
const remainingIcon = computed(() => isOverbudgeted.value ? 'report_problem' : 'savings')

// Filtered and Sorted Budgets
const filteredBudgets = computed(() => {
  let result = budgets.value.filter(b => 
    b.categoryName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    (b.categoryGroup?.toLowerCase().includes(searchQuery.value.toLowerCase()))
  )

  if (sortBy.value === 'Mayor Presupuesto') {
    result.sort((a, b) => b.limitAmount - a.limitAmount)
  } else if (sortBy.value === 'Mayor Gasto') {
    result.sort((a, b) => b.spentAmount - a.spentAmount)
  } else if (sortBy.value === 'Nombre (A-Z)') {
    result.sort((a, b) => a.categoryName.localeCompare(b.categoryName))
  }

  return result
})

const loadBudgets = async () => {
  isLoading.value = true
  try {
    const { data } = await api.get('/api/v1/budgets', {
      params: { month: selectedMonth.value, year: selectedYear.value }
    })
    budgets.value = data.budgets
    expectedIncome.value = data.monthlyIncome
  } catch (error) {
    console.error('Failed to load budgets', error)
  } finally {
    isLoading.value = false
  }
}

const openAddModal = () => {
  selectedBudget.value = null
  isModalOpen.value = true
}

const openEditModal = (budget: Budget) => {
  selectedBudget.value = budget
  isModalOpen.value = true
}

const deleteBudget = async (id: number) => {
  if (!confirm('¿Estás seguro de que deseas eliminar este presupuesto?')) return

  try {
    await api.delete(`/api/v1/budgets/${id}`)
    await loadBudgets()
  } catch (error) {
    console.error('Failed to delete budget', error)
  }
}

const getProgressColor = (spent: number, limit: number) => {
  if (limit === 0) return 'bg-primary'
  const percent = (spent / limit) * 100
  if (percent >= 100) return 'bg-red-500'
  if (percent >= 80) return 'bg-orange-500'
  return 'bg-primary'
}

onMounted(loadBudgets)
watch([selectedMonth, selectedYear], loadBudgets)
</script>

<template>
  <div class="space-y-6">
    
    <!-- Page Title -->
    <section class="space-y-1 px-1">
      <h1 class="font-headline text-3xl font-extrabold tracking-tighter text-on-surface">Planificación Mensual</h1>
      <p class="text-sm font-medium text-on-surface-variant tracking-wide uppercase">
        {{ months[selectedMonth - 1] }} {{ selectedYear }} • Análisis de Precisión
      </p>
    </section>

    <!-- Hero Budget Card: Asymmetric Bento Style -->
    <section class="grid grid-cols-1 gap-4">
      <div class="bg-[#1a1c20] rounded-lg p-6 luminous-shadow relative overflow-hidden transition-all duration-500">
        <div class="absolute top-0 right-0 w-32 h-32 bg-primary/5 blur-[60px] rounded-full -mr-16 -mt-16"></div>
        
        <div class="relative z-10 space-y-6">
          <div class="flex justify-between items-start">
            <div class="space-y-1">
              <p class="text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant">Balance por Asignar</p>
              <h2 
                class="font-headline text-5xl font-black tracking-tighter transition-colors duration-300 flex items-baseline gap-1"
                :class="remainingColorClass"
              >
                <span v-if="remainingToAllocate < 0" class="translate-y-[-0.1em]">-</span>
                <span class="whitespace-nowrap">₡{{ Math.abs(remainingToAllocate).toLocaleString() }}</span>
              </h2>
            </div>
            <div 
              class="px-3 py-1 rounded-full flex items-center gap-1 border border-white/5 transition-colors"
              :class="isOverbudgeted ? 'bg-red-500/10 text-red-500' : 'bg-primary/10 text-primary-container'"
            >
              <span class="material-symbols-outlined text-[14px]">{{ remainingIcon }}</span>
              <span class="text-[10px] font-bold uppercase">{{ remainingStatusLabel }}</span>
            </div>
          </div>

          <div class="pt-6 grid grid-cols-2 gap-8">
            <div class="space-y-1">
              <p class="text-[10px] font-bold uppercase tracking-widest text-on-surface-variant">Presupuesto Total</p>
              <p class="text-lg font-bold text-on-surface">₡{{ totalBudgeted.toLocaleString() }}</p>
            </div>
            <div class="space-y-1">
              <p class="text-[10px] font-bold uppercase tracking-widest text-on-surface-variant">Total Gastado</p>
              <p class="text-lg font-bold text-on-surface">₡{{ totalSpent.toLocaleString() }}</p>
            </div>
          </div>

          <!-- Global Progress -->
          <div class="space-y-2 pt-2">
            <div class="h-1.5 w-full bg-surface-container-highest rounded-full overflow-hidden">
              <div 
                class="h-full bg-gradient-to-r from-primary-fixed to-primary-container transition-all duration-1000 ease-out" 
                :style="{ width: `${Math.min(spentPercentage, 100)}%` }"
              ></div>
            </div>
            <div class="flex justify-between text-[10px] font-medium text-on-surface-variant uppercase tracking-tighter">
              <span>{{ spentPercentage.toFixed(1) }}% de Presupuesto Consumido</span>
              <span>₡{{ (totalBudgeted - totalSpent).toLocaleString() }} Restante</span>
            </div>
          </div>
        </div>
      </div>
    </section>


    <!-- Categories Section -->
    <section class="space-y-6">
      <div class="flex items-center justify-between px-1">
        <h3 class="font-headline text-xl font-bold tracking-tight text-on-surface">Categorías</h3>
        <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[0.2em]">{{ filteredBudgets.length }} Activas</p>
      </div>

      <div class="flex flex-col gap-4">
        <div v-if="isLoading" class="flex flex-col items-center justify-center py-20 gap-4">
          <div class="w-10 h-1 bg-surface-container-highest rounded-full overflow-hidden relative">
            <div class="absolute inset-0 bg-primary-container w-1/3 animate-[loading_1.5s_infinite]"></div>
          </div>
          <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[.2em]">Sincronizando Datos</p>
        </div>

        <div v-else-if="filteredBudgets.length === 0" class="flex flex-col items-center justify-center py-16 bg-surface-container-low rounded-2xl border border-dashed border-white/5 text-center px-6">
          <span class="material-symbols-outlined text-5xl text-on-surface-variant/20 mb-4">analytics</span>
          <p class="text-sm font-bold text-on-surface-variant mb-1">Sin registros para el mes</p>
          <p class="text-xs text-on-surface-variant/60 mb-6">Comienza tu planificación mensual agregando una categoría.</p>
          <button @click="openAddModal" class="text-[10px] font-black text-primary-container uppercase tracking-[.2em] border border-primary-container/20 px-4 py-2 rounded-lg hover:bg-primary-container/5 transition-all">Inicia Aquí</button>
        </div>

        <div v-else class="grid grid-cols-1 gap-4">
          <BudgetItemCard 
            v-for="budget in filteredBudgets" 
            :key="budget.id"
            :budget="budget"
            @edit="openEditModal"
            @delete="deleteBudget"
          />
        </div>
      </div>
    </section>

    <!-- Bottom Insight Callout -->
    <section v-if="budgets.length > 0" class="bg-surface-container-high/50 border border-white/5 rounded-2xl p-6 flex items-start gap-4 transition-all hover:bg-surface-container-high shadow-sm shadow-primary/5">
      <span class="material-symbols-outlined text-primary-container animate-pulse" style="font-variation-settings: 'FILL' 1;">auto_awesome</span>
      <div class="space-y-2">
        <h5 class="text-sm font-bold text-on-surface">Insight de Planificación</h5>
        <p class="text-xs text-on-surface-variant leading-relaxed">
          Actualmente tienes ₡{{ Math.abs(remainingToAllocate).toLocaleString() }} 
          {{ isOverbudgeted ? 'por encima de tu capacidad' : 'libres para asignar' }}.
          Recomendamos revisar tus categorías de mayor impacto para optimizar tu ahorro mensual.
        </p>
      </div>
    </section>

    <!-- Navigation Shadow element to compensate for bottom bar if any -->
    <div class="h-20 sm:hidden"></div>

    <!-- Category Budget Modal -->
    <transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <CategoryBudgetModal
        v-if="isModalOpen"
        :isOpen="isModalOpen"
        :budget="selectedBudget ? { id: selectedBudget.id, categoryId: selectedBudget.categoryId, limitAmount: selectedBudget.limitAmount } : undefined"
        :month="selectedMonth"
        :year="selectedYear"
        @close="isModalOpen = false"
        @saved="loadBudgets"
      />
    </transition>

    <!-- Floating Action Button (FAB) -->
    <button 
      @click="openAddModal"
      class="fixed bottom-32 right-8 w-14 h-14 flex items-center justify-center rounded-full bg-primary-container text-on-primary-container shadow-2xl shadow-primary-container/40 z-[100] hover:scale-110 active:scale-90 transition-all duration-300"
      aria-label="Registrar Categoría"
    >
      <span class="material-symbols-outlined text-3xl font-black">add</span>
    </button>
  </div>
</template>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 500, 'GRAD' 0, 'opsz' 24;
}

@keyframes loading {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(300%); }
}

/* Glass effect for header */
.glass-effect {
  background: rgba(17, 19, 23, 0.8);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
}
</style>

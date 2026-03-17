<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import api from '@/services/api'
import CategoryBudgetModal from '@/app/components/budgets/CategoryBudgetModal.vue'

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
const expectedIncome = ref<number>(5000) // Mock default for design alignment
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
const remainingToAllocate = computed(() => Math.max(0, expectedIncome.value - totalBudgeted.value))
const spentPercentage = computed(() => totalBudgeted.value > 0 ? (totalSpent.value / totalBudgeted.value) * 100 : 0)

// Filtered and Sorted Budgets
const filteredBudgets = computed(() => {
  let result = budgets.value.filter(b => 
    b.categoryName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    (b.categoryGroup?.toLowerCase().includes(searchQuery.value.toLowerCase()))
  )

  if (sortBy.value === 'Highest Budget') {
    result.sort((a, b) => b.limitAmount - a.limitAmount)
  } else if (sortBy.value === 'Highest Spent') {
    result.sort((a, b) => b.spentAmount - a.spentAmount)
  } else if (sortBy.value === 'Name (A-Z)') {
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
    budgets.value = data
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
  <div class="flex flex-col h-full bg-background-light dark:bg-background-dark">
    <!-- Header -->
    <header class="w-full px-6 py-6 lg:px-10 flex flex-wrap items-end justify-between gap-4 shrink-0 z-10">
      <div class="flex flex-col gap-1">
        <div class="flex items-center gap-2 text-primary text-sm font-bold tracking-wider uppercase">
          <span class="w-2 h-2 rounded-full bg-accent-lime shadow-[0_0_8px_#66CC33]"></span>
          Plan Activo
        </div>
        <div class="flex items-center gap-4">
          <h2 class="text-3xl lg:text-4xl font-extrabold tracking-tight text-slate-900 dark:text-white">
            {{ months[selectedMonth - 1] }} {{ selectedYear }}
          </h2>
          <!-- Month/Year Fast Switcher -->
          <div class="flex items-center gap-1 bg-surface-dark/10 p-1 rounded-lg border border-border-dark/10">
            <select v-model="selectedMonth" class="bg-transparent text-xs font-bold border-none outline-none cursor-pointer py-1 focus:ring-0">
              <option v-for="(m, idx) in months" :key="idx" :value="idx + 1">{{ m }}</option>
            </select>
            <select v-model="selectedYear" class="bg-transparent text-xs font-bold border-none outline-none cursor-pointer py-1 focus:ring-0">
              <option v-for="y in years" :key="y" :value="y">{{ y }}</option>
            </select>
          </div>
        </div>
        <p class="text-slate-500 dark:text-slate-400 text-sm font-medium">Gestiona tus límites de gasto y metas de ahorro.</p>
      </div>

      <div class="flex gap-3">
        <button class="flex items-center gap-2 px-4 py-2.5 rounded-lg border border-slate-200 dark:border-slate-700 hover:bg-slate-100 dark:hover:bg-surface-dark text-slate-600 dark:text-slate-300 text-sm font-bold transition-all">
          <span class="material-symbols-outlined text-xl text-primary">ios_share</span>
          <span class="hidden sm:inline">Exportar Reporte</span>
        </button>
        <button 
          @click="openAddModal"
          class="flex items-center gap-2 px-5 py-2.5 rounded-lg bg-primary hover:bg-primary/90 text-[#121416] text-sm font-bold shadow-lg shadow-primary/25 transition-all"
        >
          <span class="material-symbols-outlined text-xl">add</span>
          <span>Agregar presupuesto</span>
        </button>
      </div>
    </header>

    <!-- Scrollable Area -->
    <div class="flex-1 overflow-y-auto px-6 pb-10 lg:px-10 scroll-smooth">
      <div class="max-w-6xl mx-auto flex flex-col gap-8">
        <!-- Stats Dashboard -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 lg:gap-6">
          <!-- Total Budgeted -->
          <div class="p-6 rounded-xl bg-surface-light dark:bg-card-dark shadow-sm border border-slate-100 dark:border-slate-800 flex flex-col gap-4 relative overflow-hidden group">
            <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
              <span class="material-symbols-outlined text-6xl text-primary">account_balance_wallet</span>
            </div>
            <div>
              <p class="text-slate-500 dark:text-slate-400 text-sm font-semibold uppercase tracking-wider">Total Presupuestado</p>
              <p class="text-3xl font-extrabold text-slate-900 dark:text-white mt-1">₡{{ totalBudgeted.toLocaleString() }}</p>
            </div>
            <div class="w-full bg-slate-100 dark:bg-slate-700 h-1.5 rounded-full mt-auto">
              <div class="bg-primary/50 h-1.5 rounded-full" style="width: 100%"></div>
            </div>
          </div>

          <!-- Total Spent -->
          <div class="p-6 rounded-xl bg-surface-light dark:bg-card-dark shadow-sm border border-slate-100 dark:border-slate-800 flex flex-col gap-4 relative overflow-hidden group">
            <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
              <span class="material-symbols-outlined text-6xl text-orange-400">credit_card</span>
            </div>
            <div>
              <p class="text-slate-500 dark:text-slate-400 text-sm font-semibold uppercase tracking-wider">Total Gastado</p>
              <p class="text-3xl font-extrabold text-slate-900 dark:text-white mt-1">₡{{ totalSpent.toLocaleString() }}</p>
            </div>
            <div class="w-full bg-slate-100 dark:bg-slate-700 h-1.5 rounded-full mt-auto">
              <div class="bg-orange-400 h-1.5 rounded-full" :style="{ width: `${Math.min(spentPercentage, 100)}%` }"></div>
            </div>
          </div>

          <!-- Remaining -->
          <div class="p-6 rounded-xl bg-surface-light dark:bg-card-dark shadow-sm border border-slate-100 dark:border-slate-800 flex flex-col gap-4 relative overflow-hidden group">
            <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
              <span class="material-symbols-outlined text-6xl text-accent-lime">savings</span>
            </div>
            <div>
              <p class="text-slate-500 dark:text-slate-400 text-sm font-semibold uppercase tracking-wider">Por Asignar</p>
              <p class="text-3xl font-extrabold text-accent-lime mt-1">₡{{ remainingToAllocate.toLocaleString() }}</p>
            </div>
            <div class="flex items-center gap-2 mt-auto">
              <span class="bg-accent-lime/10 text-accent-lime text-xs font-bold px-2 py-1 rounded">En Orden</span>
              <span class="text-slate-400 text-xs">{{ ((remainingToAllocate/expectedIncome)*100).toFixed(0) }}% de ingresos libres</span>
            </div>
          </div>
        </div>

        <!-- Filter & Sort -->
        <div class="flex flex-col sm:flex-row items-center justify-between gap-4 mt-2">
          <div class="relative w-full sm:w-auto">
            <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-slate-400">search</span>
            <input 
              v-model="searchQuery"
              class="pl-10 pr-4 py-2 rounded-lg bg-surface-light dark:bg-card-dark border border-slate-200 dark:border-slate-700 text-sm text-slate-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary w-full sm:w-64" 
              placeholder="Buscar categorías..." 
              type="text"
            />
          </div>
          <div class="flex items-center gap-2 w-full sm:w-auto justify-end">
            <span class="text-xs font-semibold text-slate-500 uppercase tracking-wide">Ordenar:</span>
            <select v-model="sortBy" class="bg-transparent text-sm font-bold text-slate-700 dark:text-slate-200 focus:outline-none cursor-pointer">
              <option>Highest Budget</option>
              <option>Highest Spent</option>
              <option>Name (A-Z)</option>
            </select>
          </div>
        </div>

        <!-- Categories List -->
        <div class="flex flex-col gap-3 min-h-[400px]">
          <!-- Header Row -->
          <div class="hidden md:grid grid-cols-12 gap-4 px-4 py-2 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
            <div class="col-span-4">Categoría</div>
            <div class="col-span-4">Progreso</div>
            <div class="col-span-3 text-right">Gastado / Límite</div>
            <div class="col-span-1 text-right">Acciones</div>
          </div>

          <div v-if="isLoading" class="flex flex-col items-center justify-center py-20 gap-4">
            <div class="animate-spin size-8 border-4 border-primary border-t-transparent rounded-full"></div>
            <p class="text-text-muted">Cargando presupuestos...</p>
          </div>

          <div v-else-if="filteredBudgets.length === 0" class="flex flex-col items-center justify-center py-20 bg-card-dark rounded-xl border border-dashed border-border-dark">
            <span class="material-symbols-outlined text-6xl text-text-muted mb-4 opacity-20">inventory_2</span>
            <p class="text-text-muted font-bold">No hay presupuestos registrados para este mes.</p>
            <button @click="openAddModal" class="mt-4 text-primary font-bold hover:underline">Registrar el primero</button>
          </div>

          <!-- Items -->
          <div 
            v-for="budget in filteredBudgets" 
            :key="budget.id"
            class="group bg-card-light dark:bg-card-dark rounded-xl p-4 shadow-sm border border-transparent hover:border-primary/30 transition-all duration-200"
          >
            <div class="grid grid-cols-1 md:grid-cols-12 gap-4 items-center">
              <div class="col-span-1 md:col-span-4 flex items-center gap-4">
                <div class="w-12 h-12 rounded-lg bg-primary/10 text-primary flex items-center justify-center shrink-0">
                  <span class="material-symbols-outlined">{{ budget.categoryIcon || 'category' }}</span>
                </div>
                <div class="flex flex-col">
                  <h3 class="text-base font-bold text-slate-900 dark:text-white">{{ budget.categoryName }}</h3>
                  <p class="text-xs text-slate-500 dark:text-slate-400">{{ budget.categoryGroup }}</p>
                </div>
              </div>

              <!-- Progress -->
              <div class="col-span-1 md:col-span-4 flex flex-col justify-center gap-1.5">
                <div class="flex justify-between md:hidden text-sm font-medium">
                  <span>Progreso</span>
                  <span :class="budget.spentAmount > budget.limitAmount ? 'text-red-500' : 'text-primary'">
                    {{ ((budget.spentAmount/budget.limitAmount)*100).toFixed(0) }}%
                  </span>
                </div>
                <div class="w-full h-2.5 bg-slate-100 dark:bg-slate-700 rounded-full overflow-hidden">
                  <div 
                    class="h-full transition-all duration-500"
                    :class="getProgressColor(budget.spentAmount, budget.limitAmount)"
                    :style="{ width: `${Math.min((budget.spentAmount/budget.limitAmount)*100, 100)}%` }"
                  ></div>
                </div>
              </div>

              <!-- Amounts -->
              <div class="col-span-1 md:col-span-3 flex md:justify-end items-baseline gap-1">
                <span class="text-lg font-bold" :class="budget.spentAmount > budget.limitAmount ? 'text-red-500' : 'text-slate-900 dark:text-white'">
                  ₡{{ budget.spentAmount.toLocaleString() }}
                </span>
                <span class="text-sm text-slate-400">/ ₡{{ budget.limitAmount.toLocaleString() }}</span>
              </div>

              <!-- Actions -->
              <div class="col-span-1 flex justify-end gap-2 opacity-100 md:opacity-0 group-hover:opacity-100 transition-opacity">
                <button @click="openEditModal(budget)" class="p-2 rounded-lg text-slate-400 hover:text-primary transition-colors">
                  <span class="material-symbols-outlined text-xl">edit</span>
                </button>
                <button @click="deleteBudget(budget.id)" class="p-2 rounded-lg text-slate-400 hover:text-red-500 transition-colors">
                  <span class="material-symbols-outlined text-xl">delete</span>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Insight -->
        <div class="mt-4 p-6 rounded-xl bg-gradient-to-r from-primary/10 to-accent-lime/10 border border-primary/20 flex flex-col sm:flex-row items-center justify-between gap-6">
          <div class="flex items-start gap-4">
            <div class="w-10 h-10 rounded-full bg-gradient-to-br from-primary to-accent-lime flex items-center justify-center text-white shrink-0 shadow-lg shadow-primary/20">
              <span class="material-symbols-outlined text-xl">auto_awesome</span>
            </div>
            <div>
              <h4 class="text-lg font-bold text-slate-900 dark:text-white">Perspectiva Inteligente</h4>
              <p class="text-sm text-slate-600 dark:text-slate-300 mt-1 max-w-lg">
                Basado en tus patrones de gasto recurrentes, podrías exceder tu límite de comida este mes por ₡15,000. Considera ajustar ahora.
              </p>
            </div>
          </div>
          <button class="whitespace-nowrap px-4 py-2 rounded-lg bg-card-light dark:bg-background-dark text-slate-900 dark:text-white font-bold text-sm shadow-sm hover:shadow-md transition-all border border-transparent hover:border-primary/20">
            Ajustar Presupuesto
          </button>
        </div>
      </div>
    </div>

    <!-- Modal -->
    <CategoryBudgetModal 
      :isOpen="isModalOpen"
      :month="selectedMonth"
      :year="selectedYear"
      :budget="selectedBudget ? { id: selectedBudget.id, categoryId: selectedBudget.categoryId, limitAmount: selectedBudget.limitAmount } : undefined"
      @close="isModalOpen = false"
      @saved="loadBudgets"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'

interface Category {
  id: number
  name: string
  icon?: string
}

interface BudgetEntry {
  id: number
  categoryId: number
  categoryName: string
  limitAmount: number
}

const route = useRoute()
const router = useRouter()

const isEditMode = computed(() => !!route.params.id)
const budgetId = computed(() => route.params.id ? parseInt(route.params.id as string, 10) : null)

const categories = ref<Category[]>([])
const selectedCategoryId = ref<number | null>(null)
const limitAmount = ref<number | null>(null)
const isLoading = ref(true)
const isSubmitting = ref(false)

// Use current date for context unless provided
const month = ref(parseInt(route.query.month as string) || new Date().getMonth() + 1)
const year = ref(parseInt(route.query.year as string) || new Date().getFullYear())

const fetchCategories = async () => {
  try {
    const { data } = await api.get('/api/v1/expensecategories')
    categories.value = data
  } catch (error) {
    console.error('Failed to load categories', error)
  }
}

const fetchBudgetDetails = async () => {
  if (!budgetId.value) return
  
  try {
    isLoading.value = true
    // Fetch budget list for the specific month/year and find the one with the ID
    const { data } = await api.get('/api/v1/budgets', {
      params: { month: month.value, year: year.value }
    })
    
    const budget = data.budgets.find((b: { id: number }) => b.id === budgetId.value)
    if (budget) {
      selectedCategoryId.value = budget.categoryId
      limitAmount.value = budget.limitAmount
    } else {
      console.warn('Budget not found in this month context')
      router.replace('/app/budgets')
    }
  } catch (error) {
    console.error('Failed to fetch budget details', error)
  } finally {
    isLoading.value = false
  }
}

const handleSave = async () => {
  if (!selectedCategoryId.value || limitAmount.value === null) return

  isSubmitting.value = true
  try {
    await api.post('/api/v1/budgets/batch', {
      month: month.value,
      year: year.value,
      budgets: [{
        categoryId: selectedCategoryId.value,
        amount: limitAmount.value
      }]
    })
    router.push('/app/budgets')
  } catch (error) {
    console.error('Failed to save budget', error)
  } finally {
    isSubmitting.value = false
  }
}

const handleDelete = async () => {
  if (!budgetId.value) return
  
  if (confirm('¿Estás seguro de que deseas eliminar este presupuesto? Esta acción es permanente.')) {
    try {
      isSubmitting.value = true
      await api.delete(`/api/v1/budgets/${budgetId.value}`)
      router.push('/app/budgets')
    } catch (error) {
      console.error('Failed to delete budget', error)
    } finally {
      isSubmitting.value = false
    }
  }
}

onMounted(async () => {
  await fetchCategories()
  if (isEditMode.value) {
    await fetchBudgetDetails()
  } else {
    isLoading.value = false
  }
})

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC',
    minimumFractionDigits: 0,
  }).format(value)
}

const goBack = () => router.back()

const months = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
]
</script>

<template>
  <div class="space-y-10 pb-20">
    <!-- Header Section -->
    <header class="flex items-center justify-between px-1">
      <div class="flex items-center gap-4">
        <button 
          @click="goBack"
          class="p-2 rounded-lg hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-primary-container"
          aria-label="Volver"
        >
          <span class="material-symbols-outlined">arrow_back</span>
        </button>
        <div class="space-y-1">
          <h1 class="font-headline text-2xl font-black tracking-tighter text-on-surface">
            {{ isEditMode ? 'Editar Presupuesto' : 'Configurar Presupuesto' }}
          </h1>
          <p class="text-xs font-bold text-on-surface-variant uppercase tracking-widest">
            {{ months[month - 1] }} {{ year }} • Planificación de Precisión
          </p>
        </div>
      </div>
    </header>

    <div v-if="isLoading" class="flex flex-col items-center justify-center py-20 gap-4">
      <div class="w-10 h-1 bg-surface-container-highest rounded-full overflow-hidden relative">
        <div class="absolute inset-0 bg-primary-container w-1/3 animate-[loading_1.5s_infinite]"></div>
      </div>
      <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[.2em]">Cargando Datos</p>
    </div>

    <form v-else @submit.prevent="handleSave" class="space-y-8 animate-fade-in-up">
      <!-- Hero Amount Input -->
      <section class="relative group">
        <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant mb-4 px-1">Límite Mensual</label>
        <div class="bg-surface-container-low p-10 rounded-2xl border border-white/[0.03] luminous-shadow-sm flex flex-col items-center justify-center transition-all duration-500 focus-within:bg-primary-container/[0.02] focus-within:border-primary-container/20">
          <div class="flex items-baseline gap-2">
            <span class="font-headline text-4xl font-black text-primary-container">₡</span>
            <input 
              v-model="limitAmount"
              class="bg-transparent border-none text-center font-headline text-6xl font-black tracking-tighter focus:ring-0 text-on-surface w-full max-w-[280px] placeholder:text-on-surface-variant/20" 
              placeholder="0" 
              type="number" 
              required
            />
          </div>
          <div v-if="limitAmount" class="mt-6 px-4 py-1.5 rounded-full bg-primary-container/5 border border-primary-container/10">
            <span class="text-primary-container font-bold text-[10px] uppercase tracking-widest">Equivale a {{ formatCurrency(limitAmount) }}</span>
          </div>
        </div>
      </section>

      <!-- Category Select -->
      <section class="space-y-4">
        <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Categoría del Gasto</label>
        <div class="relative group">
          <select 
            v-model="selectedCategoryId"
            :disabled="isEditMode"
            class="w-full appearance-none bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-primary-container/30 focus:ring-0 transition-all font-body text-on-surface font-semibold cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
            required
          >
            <option :value="null" disabled>Selecciona una categoría...</option>
            <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
          </select>
          <div class="absolute right-5 top-1/2 -translate-y-1/2 pointer-events-none flex items-center gap-2">
            <span class="material-symbols-outlined text-on-surface-variant text-xl">unfold_more</span>
          </div>
        </div>
      </section>

      <!-- Threshold Alert (UI Only) -->
      <section class="bg-surface-container-low p-6 rounded-2xl border border-white/[0.02]">
        <div class="flex items-center justify-between">
          <div class="flex gap-4 items-center">
            <div class="w-12 h-12 rounded-xl bg-primary-container/10 flex items-center justify-center text-primary-container">
              <span class="material-symbols-outlined text-2xl">notifications_active</span>
            </div>
            <div class="space-y-0.5">
              <h4 class="text-sm font-bold text-on-surface">Alarma de Umbral</h4>
              <p class="text-[10px] font-medium text-on-surface-variant uppercase tracking-wide">Notificar al alcanzar el 85%</p>
            </div>
          </div>
          <label class="relative inline-flex items-center cursor-pointer group">
            <input type="checkbox" checked class="sr-only peer">
            <div class="w-12 h-6 bg-surface-container-highest rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-primary-container shadow-lg group-active:scale-95"></div>
          </label>
        </div>
      </section>

      <!-- Action Buttons -->
      <div class="pt-6 space-y-4">
        <button 
          type="submit"
          :disabled="isSubmitting || !selectedCategoryId || limitAmount === null"
          class="w-full bg-primary-container text-on-primary-container font-headline font-black py-5 rounded-xl hover:brightness-110 active:scale-[0.98] transition-all shadow-xl shadow-primary-container/20 disabled:opacity-50 flex items-center justify-center gap-3"
        >
          <span v-if="isSubmitting" class="material-symbols-outlined animate-spin">sync</span>
          <span v-else class="material-symbols-outlined">{{ isEditMode ? 'check_circle' : 'add_circle' }}</span>
          {{ isSubmitting ? 'Guardando...' : (isEditMode ? 'Actualizar Presupuesto' : 'Crear Presupuesto') }}
        </button>
        
        <button 
          @click="goBack"
          type="button"
          class="w-full bg-surface-container-high text-on-surface font-headline font-bold py-5 rounded-xl hover:bg-surface-variant active:scale-[0.98] transition-all"
        >
          Cancelar
        </button>
      </div>

      <!-- Delete Action (Edit Mode Only) -->
      <div v-if="isEditMode" class="pt-12 border-t border-white/[0.03] space-y-6">
        <button 
          @click="handleDelete"
          type="button"
          :disabled="isSubmitting"
          class="w-full group flex items-center justify-center gap-3 py-4 text-red-500 font-headline font-bold hover:bg-red-500/5 rounded-xl transition-all active:scale-[0.98] disabled:opacity-50"
        >
          <span class="material-symbols-outlined">delete_sweep</span>
          Eliminar Presupuesto
        </button>
        <p class="text-center text-[10px] font-bold text-on-surface-variant/40 uppercase tracking-[0.2em]">Esta acción es definitiva</p>
      </div>
    </form>
  </div>
</template>

<style scoped>
@keyframes loading {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(300%); }
}

.luminous-shadow-sm {
  box-shadow: 0 20px 40px -20px rgba(5, 230, 153, 0.05);
}

.animate-fade-in-up {
  animation: fadeInUp 0.5s ease-out forwards;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Hide spin arrows in number input */
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
input[type=number] {
  -moz-appearance: textfield;
  appearance: textfield;
}
</style>

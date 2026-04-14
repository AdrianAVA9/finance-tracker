<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'
import CategorySelector from '@/app/components/common/CategorySelector.vue'
import ConfirmationModal from '@/app/components/common/ConfirmationModal.vue'

interface Category {
  id: number
  name: string
  icon?: string
  color?: string
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
const isRecurrent = ref(false)
const showDeleteConfirm = ref(false)
const isDeleting = ref(false)

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
      isRecurrent.value = budget.isRecurrent
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
        amount: limitAmount.value,
        isRecurrent: isRecurrent.value
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
  isDeleting.value = true
  try {
    await api.delete(`/api/v1/budgets/${budgetId.value}`)
    router.push('/app/budgets')
  } catch (error) {
    console.error('Failed to delete budget', error)
  } finally {
    isDeleting.value = false
    showDeleteConfirm.value = false
  }
}

const selectedCategory = computed(() => categories.value.find(c => c.id === selectedCategoryId.value))

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

const monthsLabels = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
]
</script>

<template>
  <div class="space-y-10 pb-20">
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      :show="showDeleteConfirm"
      title="¿Eliminar Presupuesto?"
      description="Esta acción eliminará el límite establecido para esta categoría en este mes. No borrará tus gastos, pero perderás el seguimiento del presupuesto."
      confirm-text="Eliminar Presupuesto"
      cancel-text="Mantener"
      :is-loading="isDeleting"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteConfirm = false"
    >
      <template #item-preview v-if="selectedCategory">
        <div class="flex items-center gap-4 bg-surface-container-lowest/50 p-4 rounded-xl">
          <div 
            class="w-10 h-10 rounded-full flex items-center justify-center opacity-80"
            :style="selectedCategory.color ? { backgroundColor: selectedCategory.color + '20', color: selectedCategory.color } : { backgroundColor: '#ffffff10' }"
          >
            <span class="material-symbols-outlined text-sm">{{ selectedCategory.icon || 'category' }}</span>
          </div>
          <div class="text-left flex-1">
            <p class="text-sm font-bold text-on-surface">{{ selectedCategory.name }}</p>
            <p class="text-[10px] text-on-surface-variant uppercase tracking-widest font-label">{{ monthsLabels[month - 1] }} {{ year }}</p>
          </div>
          <div class="text-right">
            <p class="text-sm font-black text-on-surface">₡{{ limitAmount?.toLocaleString() || '0' }}</p>
          </div>
        </div>
      </template>
    </ConfirmationModal>

    <div v-if="isLoading" class="flex flex-col items-center justify-center py-20 gap-4">
      <div class="w-10 h-1 bg-surface-container-highest rounded-full overflow-hidden relative">
        <div class="absolute inset-0 bg-primary-container w-1/3 animate-[loading_1.5s_infinite]"></div>
      </div>
      <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[.2em]">Cargando Datos</p>
    </div>

    <form v-else @submit.prevent="handleSave" class="space-y-8 animate-fade-in-up">
      <!-- Hero Amount Input -->
      <section class="relative group">
        <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant mb-4 px-1">Límite para {{ monthsLabels[month - 1] }}</label>
        <div class="bg-surface-container-low p-10 rounded-xl border border-white/[0.03] luminous-shadow-sm flex flex-col items-center justify-center transition-all duration-500 focus-within:bg-primary-container/[0.02] focus-within:border-primary-container/20">
          <div class="flex items-baseline gap-2">
            <span class="font-headline text-4xl font-black text-primary-container">₡</span>
            <input 
              v-model="limitAmount"
              class="bg-transparent border-none text-center font-headline text-6xl font-black tracking-tighter focus:ring-0 text-on-surface w-full max-w-[280px] placeholder:text-on-surface-variant/20" 
              placeholder="0" 
              type="number" 
              step="0.01"
              required
            />
          </div>
        </div>
      </section>

      <!-- Recurring Active Toggle -->
      <section class="bg-surface-container-low p-6 rounded-xl border border-white/[0.02]">
        <div class="flex items-center justify-between">
          <div class="flex gap-4 items-center">
            <div class="w-12 h-12 rounded-xl bg-primary-container/10 flex items-center justify-center text-primary-container">
              <span class="material-symbols-outlined text-2xl">event_repeat</span>
            </div>
            <div class="space-y-0.5">
              <h4 class="text-sm font-bold text-on-surface">Presupuesto Recurrente</h4>
              <p class="text-[10px] font-medium text-on-surface-variant uppercase tracking-wide">Activar automáticamente cada mes</p>
            </div>
          </div>
          <label class="relative inline-flex items-center cursor-pointer group">
            <input type="checkbox" v-model="isRecurrent" class="sr-only peer">
            <div class="w-12 h-6 bg-surface-container-highest rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-primary-container shadow-lg group-active:scale-95"></div>
          </label>
        </div>
      </section>

      <!-- Category Selector -->
      <section class="space-y-4">
        <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Categoría del Gasto</label>
        <CategorySelector 
          v-model="selectedCategoryId" 
          :categories="categories" 
          :disabled="isEditMode"
          placeholder="Selecciona Categoría"
        />
      </section>

      <!-- Threshold Alert (UI Only) - Removed per user request -->

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
          @click="showDeleteConfirm = true"
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

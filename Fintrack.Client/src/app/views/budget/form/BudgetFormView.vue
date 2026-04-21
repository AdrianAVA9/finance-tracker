<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'
import CategorySelector from '@/app/components/common/CategorySelector.vue'
import ConfirmationModal from '@/app/components/common/ConfirmationModal.vue'
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import AmountInputCard from '@/app/components/common/AmountInputCard.vue'
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import AppButton from '@/app/components/common/AppButton.vue'

interface Category {
  id: string
  name: string
  icon?: string
  color?: string
}

const route = useRoute()
const router = useRouter()

const isEditMode = computed(() => !!route.params.id)
const budgetId = computed(() => route.params.id as string || null)

const categories = ref<Category[]>([])
const selectedCategoryId = ref<string | null>(null)
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

    const budget = data.budgets.find((b: { id: string }) => b.id === budgetId.value)
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

    <LoadingIndicator :is-loading="isLoading" />

    <form v-if="!isLoading" @submit.prevent="handleSave" class="space-y-8 animate-fade-in-up">
      <!-- Hero Amount Input -->
      <section class="relative group">
        <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant mb-4 px-1" for="budget-limit-amount">Límite para {{ monthsLabels[month - 1] }}</label>
        <AmountInputCard v-model="limitAmount" input-id="budget-limit-amount" />
      </section>

      <!-- Recurring Active Toggle -->
      <SurfaceCard>
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
      </SurfaceCard>

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
      <div class="pt-6">
        <AppButton
          type="submit"
          variant="primary"
          :disabled="isSubmitting || !selectedCategoryId || limitAmount === null"
          :loading="isSubmitting"
          :icon="isEditMode ? 'check_circle' : 'add_circle'"
        >
          {{ isSubmitting ? 'Guardando...' : (isEditMode ? 'Actualizar Presupuesto' : 'Crear Presupuesto') }}
        </AppButton>
      </div>

      <!-- Delete Action (Edit Mode Only) -->
      <div v-if="isEditMode" class="pt-6 border-t border-white/[0.03] space-y-6">
        <AppButton
          type="button"
          variant="danger"
          icon="delete_sweep"
          :disabled="isSubmitting"
          @click="showDeleteConfirm = true"
        >
          Eliminar Presupuesto
        </AppButton>
        <p class="text-center text-[10px] font-bold text-on-surface-variant/40 uppercase tracking-[0.2em] mt-1">Esta acción es definitiva</p>
      </div>
    </form>
  </div>
</template>

<style scoped>
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
</style>

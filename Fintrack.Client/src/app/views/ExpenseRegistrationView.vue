<template>
    <div class="space-y-10 pb-20">
        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            :show="showDeleteConfirm"
            title="¿Eliminar este gasto?"
            description="Esta acción es irreversible y tu balance mensual se verá afectado inmediatamente."
            confirm-text="Eliminar Gasto"
            cancel-text="Mantener Gasto"
            :is-loading="isDeleting"
            variant="danger"
            @confirm="handleDelete"
            @cancel="showDeleteConfirm = false"
        >
            <template #item-preview v-if="initialData">
                <div class="flex items-center gap-4 bg-surface-container-lowest/50 p-4 rounded-xl">
                    <div class="w-10 h-10 rounded-full bg-surface-container-high flex items-center justify-center">
                        <span class="material-symbols-outlined text-on-surface-variant text-sm">receipt_long</span>
                    </div>
                    <div class="text-left flex-1">
                        <p class="text-sm font-bold text-on-surface">{{ initialData.merchant }}</p>
                        <p class="text-[10px] text-on-surface-variant uppercase tracking-widest font-label">{{ initialData.date }}</p>
                    </div>
                    <div class="text-right">
                        <p class="text-sm font-black text-[#FF4D4D]">-₡{{ initialData.totalAmount.toLocaleString() }}</p>
                    </div>
                </div>
            </template>
        </ConfirmationModal>

        <!-- Page Header -->
        <header class="flex items-center justify-between px-1">
            <div class="flex items-center gap-4">
                <button 
                    @click="router.back()"
                    class="p-2 rounded-lg hover:bg-surface-container-high transition-colors text-on-surface-variant"
                    aria-label="Volver"
                >
                    <span class="material-symbols-outlined">arrow_back</span>
                </button>
                <div class="space-y-1">
                    <h1 class="font-headline text-2xl font-black tracking-tighter text-on-surface">
                      {{ isEditMode ? 'Editar Gasto' : 'Registro de Gastos' }}
                    </h1>
                    <p class="text-xs font-bold text-on-surface-variant uppercase tracking-widest">
                      {{ isEditMode ? 'Actualizar Transacción' : 'Control de Egresos' }}
                    </p>
                </div>
            </div>
        </header>

        <div v-if="isLoading" class="py-20 flex flex-col items-center justify-center gap-4">
          <div class="w-10 h-1 bg-surface-container-highest rounded-full overflow-hidden relative">
            <div class="absolute inset-0 bg-primary-container w-1/3 animate-[loading_1.5s_infinite]"></div>
          </div>
          <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[.2em]">Cargando Transacción</p>
        </div>

        <div v-else class="animate-fade-in-up">
            <ExpenseRegistrationForm 
              :submitting="isSubmitting" 
              :error="errorMessage"
              :initial-data="initialData"
              @submit="handleExpenseSubmit" 
            />

            <!-- New Delete Button Location -->
            <div class="mt-8 px-1">
                <button
                    v-if="isEditMode"
                    @click="showDeleteConfirm = true"
                    type="button"
                    class="w-full bg-red-500/10 text-[#FF4D4D] font-headline font-black py-5 rounded-xl hover:bg-red-500/20 active:scale-[0.98] transition-all flex items-center justify-center gap-3 border border-red-500/10"
                >
                    <span class="material-symbols-outlined">delete</span>
                    Eliminar Transacción
                </button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import ExpenseRegistrationForm from '@/app/components/expenses/ExpenseRegistrationForm.vue'
import ConfirmationModal from '@/app/components/common/ConfirmationModal.vue'
import api from '@/services/api'

interface ExpenseDto {
  id: number
  merchant: string
  totalAmount: number
  date: string
  note?: string
  isRecurring: boolean
  items: any[]
}

const router = useRouter()
const route = useRoute()

// Edit Mode Logic
const expenseId = computed(() => {
  const idParam = route.params.id as string;
  if (!idParam) return null;
  const match = idParam.match(/\d+/);
  return match ? parseInt(match[0], 10) : null;
});
const isEditMode = computed(() => !!expenseId.value)

// State
const initialData = ref<ExpenseDto | null>(null)
const isSubmitting = ref(false)
const isLoading = ref(isEditMode.value)
const errorMessage = ref<string | undefined>(undefined)
const showDeleteConfirm = ref(false)
const isDeleting = ref(false)

const loadExpenseData = async () => {
  if (!expenseId.value) return
  isLoading.value = true
  try {
    const { data } = await api.get(`/api/v1/expenses/${expenseId.value}`)
    initialData.value = data
  } catch (error: any) {
    console.error('Error loading expense:', error)
    errorMessage.value = 'No se pudo cargar la información del gasto.'
  } finally {
    isLoading.value = false
  }
}

const handleExpenseSubmit = async (expenseData: any) => {
  isSubmitting.value = true
  errorMessage.value = undefined
  
  try {
    const payload = {
      ...expenseData,
      frequency: expenseData.recurringData?.frequency ?? null,
      nextDate: expenseData.recurringData?.nextDate ?? null
    }
    delete payload.recurringData;

    if (isEditMode.value) {
      await api.put(`/api/v1/expenses/${expenseId.value}`, payload)
    } else {
      await api.post('/api/v1/expenses', payload)
    }
    
    router.push({ name: 'Activity' }) // Redirect to activity to see the change
  } catch (error: any) {
    console.error('Error saving expense:', error)
    errorMessage.value = error.response?.data?.detail || error.message || 'Error al guardar el gasto.'
    isSubmitting.value = false
  }
}

const handleDelete = async () => {
  if (!expenseId.value) return
  isDeleting.value = true
  try {
    await api.delete(`/api/v1/expenses/${expenseId.value}`)
    router.push({ name: 'Activity' })
  } catch (error) {
    console.error('Error deleting expense:', error)
    errorMessage.value = 'Error al eliminar el gasto.'
  } finally {
    isDeleting.value = false
    showDeleteConfirm.value = false
  }
}

onMounted(() => {
  if (isEditMode.value) {
    loadExpenseData()
  }
})
</script>

<style scoped>
@keyframes loading {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(300%); }
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
</style>

<template>
  <div class="space-y-6">
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
      <template v-if="initialData" #item-preview>
        <div class="flex items-center gap-4 rounded-xl bg-surface-container-lowest/50 p-4">
          <div
            class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-surface-container-high"
          >
            <span class="material-symbols-outlined text-sm text-on-surface-variant">receipt_long</span>
          </div>
          <div class="min-w-0 flex-1 text-left">
            <p class="text-sm font-bold text-on-surface">{{ initialData.merchant }}</p>
            <p class="font-label text-[10px] font-bold uppercase tracking-widest text-on-surface-variant">
              {{ initialData.date }}
            </p>
          </div>
          <div class="shrink-0 text-right">
            <p class="text-sm font-black text-red-500">-₡{{ initialData.totalAmount.toLocaleString() }}</p>
          </div>
        </div>
      </template>
    </ConfirmationModal>

    <LoadingIndicator :is-loading="isLoading" message="Cargando Transacción" />

    <div v-if="!isLoading" class="space-y-6">
      <ExpenseRegistrationForm
        :submitting="isSubmitting"
        :error="errorMessage"
        :initial-data="initialData"
        @submit="handleExpenseSubmit"
      />

      <AppButton
        v-if="isEditMode"
        type="button"
        variant="danger"
        icon="delete"
        @click="showDeleteConfirm = true"
      >
        Eliminar Transacción
      </AppButton>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import ExpenseRegistrationForm from '@/app/components/expenses/ExpenseRegistrationForm.vue'
import ConfirmationModal from '@/app/components/common/ConfirmationModal.vue'
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import AppButton from '@/app/components/common/AppButton.vue'
import api from '@/services/api'
import type { ExpenseSubmitPayload } from '@/app/components/expenses/useExpenseRegistrationForm'

interface ExpenseDto {
  id: string
  merchant: string
  totalAmount: number
  date: string
  items: { categoryId: string; itemAmount: number; description: string }[]
}

const router = useRouter()
const route = useRoute()

const expenseId = computed((): string | null => {
  const raw = route.params.id
  if (typeof raw !== 'string' || raw.trim() === '') return null
  return raw
})

const isEditMode = computed(() => expenseId.value !== null)

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
    const { data } = await api.get<ExpenseDto>(`/api/v1/expenses/${expenseId.value}`)
    initialData.value = data
  } catch (error: unknown) {
    console.error('Error loading expense:', error)
    errorMessage.value = 'No se pudo cargar la información del gasto.'
  } finally {
    isLoading.value = false
  }
}

const handleExpenseSubmit = async (expenseData: ExpenseSubmitPayload) => {
  isSubmitting.value = true
  errorMessage.value = undefined

  try {
    if (isEditMode.value && expenseId.value) {
      await api.put(`/api/v1/expenses/${expenseId.value}`, expenseData)
    } else {
      await api.post('/api/v1/expenses', expenseData)
    }

    router.push({ name: 'Activity' })
  } catch (error: unknown) {
    console.error('Error saving expense:', error)
    const err = error as { response?: { data?: { detail?: string } }; message?: string }
    errorMessage.value = err.response?.data?.detail || err.message || 'Error al guardar el gasto.'
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

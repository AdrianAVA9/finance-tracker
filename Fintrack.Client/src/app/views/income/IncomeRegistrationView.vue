<template>
  <div class="space-y-6">
    <ConfirmationModal
      :show="showDeleteConfirm"
      title="¿Eliminar este ingreso?"
      description="Esta acción es irreversible y tu balance mensual se verá afectado inmediatamente."
      confirm-text="Eliminar Ingreso"
      cancel-text="Mantener Ingreso"
      :is-loading="isDeleting"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteConfirm = false"
    >
      <template v-if="incomeDetails" #item-preview>
        <div class="flex items-center gap-4 rounded-xl bg-surface-container-lowest/50 p-4">
          <div
            class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-surface-container-high text-primary-container"
          >
            <span class="material-symbols-outlined text-sm">payments</span>
          </div>
          <div class="min-w-0 flex-1 text-left">
            <p class="text-sm font-bold text-on-surface">{{ incomeDetails.source || 'Ingreso sin nombre' }}</p>
            <p class="font-label text-[10px] font-bold uppercase tracking-widest text-on-surface-variant">
              {{ formatDateLabel(incomeDetails.date) }}
            </p>
          </div>
          <div class="shrink-0 text-right">
            <p class="text-sm font-black text-primary-container">+₡{{ Number(incomeDetails.amount).toLocaleString() }}</p>
          </div>
        </div>
      </template>
    </ConfirmationModal>

    <LoadingIndicator :is-loading="isLoading" message="Cargando datos" />

    <div v-if="!isLoading" class="space-y-6">
      <div
        v-if="isEditMode && !incomeDetails"
        class="rounded-xl border border-red-500/20 bg-red-500/10 p-4 text-center text-sm text-red-500"
      >
        {{ errorMessage || 'No se pudo cargar el ingreso.' }}
      </div>
      <template v-else>
        <IncomeRegistrationForm
          ref="formRef"
          :submitting="isSubmitting"
          :error="errorMessage"
          :initial-data="incomeDetails"
          @submit="handleSubmit"
          @save-and-another="handleSaveAndAnother"
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
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import IncomeRegistrationForm from '@/app/components/incomes/IncomeRegistrationForm.vue'
import ConfirmationModal from '@/app/components/common/ConfirmationModal.vue'
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import AppButton from '@/app/components/common/AppButton.vue'
import api from '@/services/api'
import type { IncomeDetailsDto, IncomeSubmitPayload } from '@/app/components/incomes/useIncomeRegistrationForm'

const router = useRouter()
const route = useRoute()

const formRef = ref<InstanceType<typeof IncomeRegistrationForm> | null>(null)

const incomeId = computed((): string | null => {
  const raw = route.params.id
  if (typeof raw !== 'string' || raw.trim() === '') return null
  return raw
})

const isEditMode = computed(() => incomeId.value !== null)

const incomeDetails = ref<IncomeDetailsDto | null>(null)
const isSubmitting = ref(false)
const isLoading = ref(isEditMode.value)
const errorMessage = ref<string | undefined>(undefined)
const showDeleteConfirm = ref(false)
const isDeleting = ref(false)

function formatDateLabel(iso: string) {
  return typeof iso === 'string' ? iso.split('T')[0] : iso
}

const loadIncomeDetails = async () => {
  if (!incomeId.value) return
  isLoading.value = true
  errorMessage.value = undefined
  try {
    const { data } = await api.get<IncomeDetailsDto>(`/api/v1/incomes/${incomeId.value}`)
    incomeDetails.value = data
  } catch (error: unknown) {
    console.error('Failed to load income details:', error)
    errorMessage.value = 'No se pudo cargar el ingreso.'
  } finally {
    isLoading.value = false
  }
}

async function persistIncome(payload: IncomeSubmitPayload, navigateAfter: boolean): Promise<boolean> {
  isSubmitting.value = true
  errorMessage.value = undefined
  try {
    if (isEditMode.value && incomeId.value) {
      await api.put(`/api/v1/incomes/${incomeId.value}`, payload)
    } else {
      await api.post('/api/v1/incomes', payload)
    }
    if (navigateAfter) {
      router.push({ name: 'Activity' })
    }
    return true
  } catch (error: unknown) {
    console.error('Failed to save income:', error)
    const err = error as { response?: { data?: { detail?: string } }; message?: string }
    errorMessage.value = err.response?.data?.detail || err.message || 'Error al guardar el ingreso.'
    return false
  } finally {
    isSubmitting.value = false
  }
}

const handleSubmit = async (payload: IncomeSubmitPayload) => {
  await persistIncome(payload, true)
}

const handleSaveAndAnother = async (payload: IncomeSubmitPayload) => {
  const ok = await persistIncome(payload, false)
  if (ok) {
    formRef.value?.resetForNewEntry()
  }
}

const handleDelete = async () => {
  if (!incomeId.value) return
  isDeleting.value = true
  try {
    await api.delete(`/api/v1/incomes/${incomeId.value}`)
    router.push({ name: 'Activity' })
  } catch (error) {
    console.error('Failed to delete income:', error)
    errorMessage.value = 'Error al eliminar el ingreso.'
  } finally {
    isDeleting.value = false
    showDeleteConfirm.value = false
  }
}

onMounted(() => {
  if (isEditMode.value) {
    loadIncomeDetails()
  }
})
</script>

<template>
    <div class="space-y-10 pb-20">
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
                    <h1 class="font-headline text-2xl font-black tracking-tighter text-on-surface">Registro de Gastos</h1>
                    <p class="text-xs font-bold text-on-surface-variant uppercase tracking-widest">Control de Egresos</p>
                </div>
            </div>
        </header>

        <div class="animate-fade-in-up">
            <ExpenseRegistrationForm 
              :submitting="isSubmitting" 
              :error="errorMessage"
              @submit="handleExpenseSubmit" 
            />
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import ExpenseRegistrationForm from '@/app/components/expenses/ExpenseRegistrationForm.vue'
import api from '@/services/api'

const router = useRouter()
const isSubmitting = ref(false)
const errorMessage = ref<string | undefined>(undefined)

const handleExpenseSubmit = async (expenseData: any) => {
  isSubmitting.value = true
  errorMessage.value = undefined
  
  try {
    // Flatten frequency if present
    const payload = {
      ...expenseData,
      frequency: expenseData.recurringData?.frequency ?? null,
    }
    delete payload.recurringData;

    await api.post('/api/v1/expenses', payload)
    
    // Success feedback and redirect
    router.push({ name: 'Dashboard' })
  } catch (error: any) {
    console.error('Error saving expense:', error)
    errorMessage.value = error.response?.data?.detail || error.message || 'Error al guardar el gasto. Por favor intente de nuevo.'
    isSubmitting.value = false
  }
}
</script>

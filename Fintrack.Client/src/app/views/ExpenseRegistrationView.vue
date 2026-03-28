<template>
    <div class="w-full pb-6">
        <div class="bg-white dark:bg-card-dark border border-slate-200 dark:border-slate-800 rounded-2xl shadow-soft overflow-hidden">
            <!-- Header -->
            <div class="p-5 border-b border-slate-100 dark:border-slate-800">
                <h1 class="text-xs font-bold tracking-widest text-[#FF4D4D] uppercase mb-1">Registro de Gastos</h1>
                <p class="text-[11px] text-slate-500 dark:text-text-secondary-dark">Complete los detalles de su nuevo gasto.</p>
            </div>
            
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

<template>
  <div class="max-w-4xl mx-auto pb-8 px-4 sm:px-6 lg:px-8">
    <ExpenseRegistrationForm @submit="handleExpenseSubmit" />
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import ExpenseRegistrationForm from '@/app/components/expenses/ExpenseRegistrationForm.vue'
import api from '@/services/api'

const router = useRouter()

const handleExpenseSubmit = async (expenseData: any) => {
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
  } catch (error) {
    console.error('Error saving expense:', error)
    alert('Error al guardar el gasto. Por favor intente de nuevo.')
  }
}
</script>

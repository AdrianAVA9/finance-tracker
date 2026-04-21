<script setup lang="ts">
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import BudgetDetailsMonthlyChart from '@/app/components/budget/BudgetDetailsMonthlyChart.vue'
import BudgetDetailsMonthlyHistory from '@/app/components/budget/BudgetDetailsMonthlyHistory.vue'
import BudgetDetailsYtdSummary from '@/app/components/budget/BudgetDetailsYtdSummary.vue'
import { useBudgetDetails } from './composables/useBudgetDetails'

const {
  budgetData,
  chartData,
  isLoading,
  maxExpense,
  openMonthIndex,
  toggleMonth,
  totalSpentYTD,
} = useBudgetDetails()
</script>

<template>
  <div>
    <LoadingIndicator :is-loading="isLoading" message="Cargando detalles..." />

    <div v-if="budgetData && !isLoading" class="space-y-8 pb-10">
      <BudgetDetailsYtdSummary
        :category-name="budgetData.categoryName"
        :total-spent-ytd="totalSpentYTD"
      />

      <BudgetDetailsMonthlyChart :chart-data="chartData" :max-expense="maxExpense" />

      <BudgetDetailsMonthlyHistory
        :monthly-history="budgetData.monthlyHistory"
        :limit-amount="budgetData.limitAmount"
        :open-month-index="openMonthIndex"
        @toggle="toggleMonth"
      />
    </div>

    <div
      v-else-if="!isLoading && !budgetData"
      class="flex flex-col items-center justify-center min-h-[400px]"
    >
      <span class="material-symbols-outlined text-[#FF4D4D] text-5xl mb-4">error</span>
      <h4 class="text-lg font-bold text-[#F3F4F6] font-headline">Presupuesto no encontrado</h4>
      <p class="text-[#bacbbe] text-sm font-label">Retrocede e intenta nuevamente.</p>
    </div>
  </div>
</template>

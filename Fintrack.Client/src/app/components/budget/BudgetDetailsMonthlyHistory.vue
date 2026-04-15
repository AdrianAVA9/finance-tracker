<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import type { MonthlyExpenseSummaryDto } from '@/services/budgetService'
import {
  formatBudgetCurrency,
  formatBudgetExpenseDate,
  getMonthShortLabel,
} from '@/app/components/budget/budgetDetailsFormatters'

const props = defineProps<{
  monthlyHistory: MonthlyExpenseSummaryDto[]
  limitAmount: number
  openMonthIndex: number | null
}>()

const emit = defineEmits<{
  toggle: [index: number]
}>()

const spentPercent = (monthData: MonthlyExpenseSummaryDto): number => {
  if (props.limitAmount <= 0) return 0
  return Math.round((monthData.totalExpense / props.limitAmount) * 100)
}
</script>

<template>
  <section class="space-y-4">
    <h3 class="font-headline font-bold text-[10px] uppercase tracking-[0.2em] text-[#bacbbe]">Historial Detallado</h3>

    <div class="space-y-3">
      <SurfaceCard
        v-for="(monthData, index) in monthlyHistory"
        :key="`${monthData.month}-${monthData.year}`"
        class="!rounded-lg overflow-hidden transition-all duration-300 !p-0"
        :class="[
          openMonthIndex === index
            ? '!bg-[#282a2e] shadow-2xl border-[#05e699]/10'
            : '!bg-[#1e2024] hover:!bg-[#222428]',
        ]"
      >
        <button
          type="button"
          class="w-full flex items-center justify-between p-4 text-left"
          @click="emit('toggle', index)"
        >
          <div>
            <span class="block font-headline font-bold text-[#e2e2e8] capitalize">
              {{ getMonthShortLabel(monthData.month) }} {{ monthData.year }}
            </span>
            <span class="block text-xs font-medium text-[#bacbbe]"> {{ spentPercent(monthData) }}% gastado </span>
          </div>
          <div class="flex items-center gap-4">
            <span class="font-headline font-bold text-[#F3F4F6]">
              {{ formatBudgetCurrency(monthData.totalExpense) }}
            </span>
            <span
              class="material-symbols-outlined text-[#bacbbe] transition-transform duration-300"
              :class="{ 'rotate-180': openMonthIndex === index }"
            >
              expand_more
            </span>
          </div>
        </button>

        <div v-if="openMonthIndex === index" class="px-4 pb-4 space-y-2 border-t border-white/5 pt-3">
          <div v-if="monthData.expenses.length === 0" class="text-center py-4 text-[#bacbbe] text-sm font-label">
            No hay gastos registrados este mes.
          </div>
          <div v-for="expense in monthData.expenses" :key="expense.id" class="flex justify-between items-center py-2">
            <div class="truncate mr-4">
              <p class="text-sm font-bold text-[#e2e2e8] truncate">{{ expense.description }}</p>
              <p class="text-xs text-[#bacbbe]">{{ formatBudgetExpenseDate(expense.date) }}</p>
            </div>
            <span class="text-sm font-bold text-[#FF4D4D] whitespace-nowrap">
              -{{ formatBudgetCurrency(expense.amount) }}
            </span>
          </div>
        </div>
      </SurfaceCard>
    </div>
  </section>
</template>

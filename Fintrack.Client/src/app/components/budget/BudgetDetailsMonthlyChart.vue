<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import type { MonthlyExpenseSummaryDto } from '@/services/budgetService'
import {
  formatBudgetCurrency,
  getMonthShortLabel,
} from '@/app/components/budget/budgetDetailsFormatters'

defineProps<{
  chartData: MonthlyExpenseSummaryDto[]
  maxExpense: number
}>()
</script>

<template>
  <SurfaceCard class="!p-5 flex flex-col gap-4 relative overflow-hidden">
    <div
      class="absolute inset-0 pointer-events-none"
      style="background: linear-gradient(180deg, rgba(5, 230, 153, 0.08) 0%, rgba(5, 230, 153, 0) 100%)"
    />
    <h3 class="font-headline font-bold text-lg text-[#F3F4F6] relative z-10">Resumen 12 Meses</h3>

    <div class="relative h-48 w-full pt-6 z-10">
      <div class="absolute top-0 left-0 w-full flex items-center gap-2 opacity-50 z-0">
        <span class="text-[10px] font-label">{{ formatBudgetCurrency(maxExpense) }}</span>
        <div class="h-px flex-1 border-dashed border-b border-[#bacbbe]/30" />
      </div>

      <div class="flex items-end justify-between gap-1 h-full w-full relative z-10">
        <div
          v-for="(item, index) in chartData"
          :key="`${item.month}-${item.year}-${index}`"
          class="flex flex-col items-center flex-1 h-full justify-end group cursor-pointer relative"
        >
          <div
            class="opacity-0 group-hover:opacity-100 transition-opacity absolute -top-10 bg-[#333539] text-[#e2e2e8] text-[10px] py-1 px-2 rounded font-label whitespace-nowrap z-20 pointer-events-none mb-1 shadow-lg border border-[rgba(255,255,255,0.05)]"
          >
            {{ formatBudgetCurrency(item.totalExpense) }}
          </div>

          <div
            class="min-h-[4px] w-full max-w-[20px] rounded-t-sm flex items-end justify-center relative transition-all duration-300 group-hover:-translate-y-1 shadow-[0_0_10px_rgba(5,230,153,0.1)]"
            :class="item.totalExpense > 0 ? 'bg-[#05e699]' : 'bg-[#333539]'"
            :style="{
              height: maxExpense > 0 ? `${(item.totalExpense / maxExpense) * 100}%` : '0px',
            }"
          />

          <span
            class="text-[10px] font-label uppercase mt-2 opacity-80"
            :class="{ 'font-bold text-[#F3F4F6] opacity-100': item.totalExpense > 0 }"
          >
            {{ getMonthShortLabel(item.month).charAt(0) }}
          </span>
        </div>
      </div>
    </div>
  </SurfaceCard>
</template>

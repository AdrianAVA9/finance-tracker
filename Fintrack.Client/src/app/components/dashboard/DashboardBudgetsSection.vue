<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import type { BudgetSummaryDto } from '@/services/dashboardService'
import { formatDashboardCurrency } from '@/app/views/dashboard/utils/dashboardFormatters'

defineProps<{
  budgets: BudgetSummaryDto[]
}>()

</script>

<template>
  <section class="space-y-4">
    <div class="flex justify-between items-center">
      <h3 class="font-headline font-bold text-xl tracking-tight">Presupuestos</h3>
    </div>

    <div class="space-y-2">
      <router-link
        v-for="budget in budgets"
        :key="budget.id"
        :to="`/app/budgets/${budget.id}`"
        class="block cursor-pointer group"
      >
        <SurfaceCard class="!p-3.5 !rounded-xl space-y-3 transition-colors hover:bg-[#222428]">
          <div class="flex justify-between items-center">
            <div class="flex items-center gap-3">
              <div
                class="w-10 h-10 rounded-xl flex items-center justify-center bg-[#333539]"
                :style="budget.color ? { backgroundColor: `${budget.color}25` } : {}"
              >
                <span
                  class="material-symbols-outlined text-[20px]"
                  :style="budget.color ? { color: budget.color } : { color: '#bacbbe' }"
                >
                  {{ budget.icon || 'category' }}
                </span>
              </div>
              <div>
                <h4 class="font-bold text-sm text-[#e2e2e8] group-hover:text-white transition-colors">
                  {{ budget.categoryName }}
                </h4>
                <p class="text-xs text-[#bacbbe] mt-0.5">
                  {{ formatDashboardCurrency(budget.remainingAmount) }} restantes
                </p>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <span
                class="font-headline font-bold text-sm"
                :class="budget.percentage > 100 ? 'text-[#FF4D4D]' : 'text-[#05E699]'"
              >
                {{ Math.round(budget.percentage) }}%
              </span>
              <span
                class="material-symbols-outlined text-[#bacbbe] text-sm opacity-0 transition-all -translate-x-2 group-hover:opacity-100 group-hover:translate-x-0"
              >
                chevron_right
              </span>
            </div>
          </div>

          <div class="w-full bg-[#333539] h-1.5 rounded-full overflow-hidden">
            <div
              class="h-full rounded-full transition-all duration-500"
              :style="{
                width: `${Math.min(budget.percentage, 100)}%`,
                backgroundColor: budget.percentage > 100 ? '#FF4D4D' : '#05E699',
              }"
            />
          </div>
        </SurfaceCard>
      </router-link>

      <SurfaceCard v-if="budgets.length === 0" class="!p-4 !rounded-xl text-center border border-white/5">
        <p class="text-[#bacbbe] text-sm">No active budgets for this month.</p>
      </SurfaceCard>
    </div>
  </section>
</template>

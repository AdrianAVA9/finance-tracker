<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import type { TransactionDto } from '@/services/dashboardService'
import {
  formatDashboardCurrency,
  formatDashboardDate,
} from '@/app/views/dashboard/utils/dashboardFormatters'

defineProps<{
  transactions: TransactionDto[]
}>()

</script>

<template>
  <section class="space-y-4">
    <h3 class="font-headline font-bold text-xl tracking-tight">Actividad Reciente</h3>

    <div class="space-y-1">
      <SurfaceCard
        v-for="transaction in transactions"
        :key="transaction.id"
        class="!p-4 !rounded-xl"
      >
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-4 max-w-[60%]">
            <div class="w-12 h-12 rounded-xl bg-[#1e2024] flex items-center justify-center shrink-0">
              <span
                class="material-symbols-outlined text-white"
                :style="{ color: transaction.categoryColor || '#fff' }"
              >
                {{ transaction.categoryIcon || 'payments' }}
              </span>
            </div>

            <div class="truncate">
              <p class="font-bold text-sm truncate">{{ transaction.description }}</p>
              <p class="text-xs text-[#bacbbe]">
                {{ transaction.categoryName }} • {{ formatDashboardDate(transaction.date) }}
              </p>
            </div>
          </div>

          <span
            :class="[
              'font-headline font-bold text-sm sm:text-base',
              transaction.amount >= 0 ? 'text-[#05E699]' : 'text-[#FF4D4D]',
            ]"
          >
            {{ transaction.amount >= 0 ? '+' : '' }}{{ formatDashboardCurrency(transaction.amount) }}
          </span>
        </div>
      </SurfaceCard>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import { formatDashboardCurrency } from '@/app/views/dashboard/utils/dashboardFormatters'

const props = defineProps<{
  totalBalance: number
  monthlyIncome: number
  monthlyExpenses: number
}>()

const totalBalanceClass = computed(() =>
  props.totalBalance < 0 ? 'text-[#FF4D4D]' : 'text-[#F3F4F6]',
)
</script>

<template>
  <section class="space-y-1">
    <p class="text-[#bacbbe] font-label text-xs uppercase tracking-[0.2em] font-medium">Balance Total</p>
    <h1 class="text-4xl font-extrabold font-headline tracking-tight" :class="totalBalanceClass">
      {{ formatDashboardCurrency(totalBalance) }}
    </h1>

    <div class="grid grid-cols-2 gap-3 pt-4">
      <SurfaceCard class="!p-3 !rounded-lg border-l-[3px] border-[#05E699]">
        <div class="flex flex-col justify-center min-w-0">
          <span class="text-[10px] text-[#bacbbe] uppercase font-bold tracking-widest">Ingresos</span>
          <div class="flex items-center gap-1 mt-0.5">
            <span class="material-symbols-outlined text-[#05E699] text-[16px]">arrow_upward</span>
            <span class="text-[#05E699] font-headline font-bold text-sm sm:text-base truncate">
              {{ formatDashboardCurrency(monthlyIncome) }}
            </span>
          </div>
        </div>
      </SurfaceCard>

      <SurfaceCard class="!p-3 !rounded-lg border-l-[3px] border-[#FF4D4D]">
        <div class="flex flex-col justify-center min-w-0">
          <span class="text-[10px] text-[#bacbbe] uppercase font-bold tracking-widest">Gastos</span>
          <div class="flex items-center gap-1 mt-0.5">
            <span class="material-symbols-outlined text-[#FF4D4D] text-[16px]">arrow_downward</span>
            <span class="text-[#FF4D4D] font-headline font-bold text-sm sm:text-base truncate">
              -{{ formatDashboardCurrency(monthlyExpenses) }}
            </span>
          </div>
        </div>
      </SurfaceCard>
    </div>
  </section>
</template>

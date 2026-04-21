<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import { formatBudgetCurrency } from '@/app/components/budget/budgetDetailsFormatters'

defineProps<{
  categoryName: string
  totalSpentYtd: number
}>()
</script>

<template>
  <section class="space-y-4">
    <h1 class="text-3xl font-extrabold font-headline tracking-tight text-[#F3F4F6]">
      {{ categoryName }}
    </h1>

    <SurfaceCard class="!p-4 flex items-center justify-between mt-6 shadow-sm shadow-black/20">
      <span class="text-[#bacbbe] font-label text-[11px] font-bold tracking-[0.1em] uppercase">
        Total Gastado Este Año
      </span>
      <span class="text-[#05e699] font-headline font-semibold text-xl">
        {{ formatBudgetCurrency(totalSpentYtd) }}
      </span>
    </SurfaceCard>
  </section>
</template>

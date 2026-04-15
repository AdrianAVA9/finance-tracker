<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import type { TransactionDto } from '@/services/dashboardService'

defineProps<{
  transactions: TransactionDto[]
}>()

const formatCurrency = (value: number): string =>
  new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(value)

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  const now = new Date()
  const isToday = date.toDateString() === now.toDateString()

  if (isToday) {
    return `Hoy, ${date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`
  }

  return (
    date.toLocaleDateString('es-ES', {
      month: 'short',
      day: '2-digit',
      year: date.getFullYear() !== now.getFullYear() ? 'numeric' : undefined,
    }) + `, ${date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`
  )
}
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
              <p class="text-xs text-[#bacbbe]">{{ transaction.categoryName }} • {{ formatDate(transaction.date) }}</p>
            </div>
          </div>

          <span
            :class="[
              'font-headline font-bold text-sm sm:text-base',
              transaction.amount >= 0 ? 'text-[#05E699]' : 'text-[#FF4D4D]',
            ]"
          >
            {{ transaction.amount >= 0 ? '+' : '' }}{{ formatCurrency(transaction.amount) }}
          </span>
        </div>
      </SurfaceCard>
    </div>
  </section>
</template>

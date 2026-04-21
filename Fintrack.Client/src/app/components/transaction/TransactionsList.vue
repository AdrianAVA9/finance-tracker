<script setup lang="ts">
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import type { TransactionDto } from '@/services/transactionService'

interface GroupedTransactions {
  date: string
  items: TransactionDto[]
}

defineProps<{
  groups: GroupedTransactions[]
  formatCurrency: (amount: number) => string
}>()

const emit = defineEmits<{
  (event: 'select-transaction', value: TransactionDto): void
}>()
</script>

<template>
  <section class="space-y-12">
    <div v-for="group in groups" :key="group.date" class="space-y-6">
      <div class="flex items-center gap-4 px-1">
        <h3 class="font-headline whitespace-nowrap text-[10px] font-black uppercase tracking-[0.3em] text-on-surface-variant">
          {{ group.date }}
        </h3>
        <div class="h-px w-full bg-white/[0.03]"></div>
      </div>

      <div class="space-y-3">
        <SurfaceCard
          v-for="transaction in group.items"
          :key="transaction.id"
          class="group cursor-pointer !p-5 transition-all duration-300 hover:-translate-y-1 hover:bg-surface-container"
          @click="emit('select-transaction', transaction)"
        >
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-5">
              <div
                class="flex h-12 w-12 items-center justify-center rounded-xl bg-surface-container transition-colors group-hover:bg-surface-container-high"
                :style="transaction.categoryColor ? { color: transaction.categoryColor } : { color: '#F3F4F6' }"
              >
                <span class="material-symbols-outlined text-2xl">
                  {{ transaction.categoryIcon || (transaction.type === 'Income' ? 'payments' : 'receipt_long') }}
                </span>
              </div>
              <div class="space-y-0.5">
                <p class="font-headline text-sm font-bold leading-tight text-on-surface">{{ transaction.description }}</p>
                <p class="text-[10px] font-black uppercase tracking-widest text-on-surface-variant/60">
                  {{ transaction.categoryName }}
                </p>
              </div>
            </div>
            <div class="space-y-0.5 text-right">
              <p
                class="font-headline text-sm font-black"
                :class="transaction.type === 'Income' ? 'text-[#05E699]' : 'text-[#FF4D4D]'"
              >
                {{ transaction.type === 'Income' ? '+' : '' }}{{ formatCurrency(transaction.amount) }}
              </p>
              <p class="text-[9px] font-bold uppercase text-on-surface-variant/30">
                {{ transaction.type === 'Income' ? 'Abono Detectado' : 'Débito Procesado' }}
              </p>
            </div>
          </div>
        </SurfaceCard>
      </div>
    </div>
  </section>
</template>

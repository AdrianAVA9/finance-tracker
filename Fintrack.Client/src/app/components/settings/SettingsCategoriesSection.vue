<script setup lang="ts">
const emit = defineEmits<{
  expenseCategories: [];
  incomeCategories: [];
}>();
</script>

<template>
  <section class="space-y-4">
    <h3
      class="font-headline text-xs font-black uppercase tracking-[0.2em] text-on-surface-variant px-1"
    >
      Categorías
    </h3>
    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
      <button
        type="button"
        class="p-5 bg-surface-container-low rounded-xl border-l-4 border-error text-left hover:bg-surface-container-high transition-all"
        @click="emit('expenseCategories')"
      >
        <div class="flex justify-between items-start mb-4">
          <span class="material-symbols-outlined text-error" style="font-variation-settings: 'FILL' 1"
            >payments</span
          >
          <span class="material-symbols-outlined text-on-surface-variant text-sm">north_east</span>
        </div>
        <p class="font-headline font-bold text-lg text-on-surface">Categorías de gastos</p>
        <p class="text-xs text-on-surface-variant mt-1">Administra tus rubros de gasto</p>
      </button>
      <button
        type="button"
        class="p-5 bg-surface-container-low rounded-xl border-l-4 border-primary-container text-left hover:bg-surface-container-high transition-all"
        @click="emit('incomeCategories')"
      >
        <div class="flex justify-between items-start mb-4">
          <span
            class="material-symbols-outlined text-primary-container"
            style="font-variation-settings: 'FILL' 1"
            >account_balance_wallet</span
          >
          <span class="material-symbols-outlined text-on-surface-variant text-sm">north_east</span>
        </div>
        <p class="font-headline font-bold text-lg text-on-surface">Categorías de ingresos</p>
        <p class="text-xs text-on-surface-variant mt-1">Sigue las fuentes de ingreso</p>
      </button>
    </div>
  </section>
</template>

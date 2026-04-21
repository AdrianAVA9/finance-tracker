<script setup lang="ts">
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import TransactionsEmptyState from '@/app/components/transaction/TransactionsEmptyState.vue'
import TransactionsFilters from '@/app/components/transaction/TransactionsFilters.vue'
import TransactionsHeader from '@/app/components/transaction/TransactionsHeader.vue'
import TransactionsInsightCard from '@/app/components/transaction/TransactionsInsightCard.vue'
import TransactionsList from '@/app/components/transaction/TransactionsList.vue'
import { useTransactionsView } from '@/app/components/transaction/useTransactionsView'

const {
  formatCurrency,
  groupedTransactions,
  isLoading,
  isSearchOpen,
  navigateToEdit,
  searchQuery,
  selectType,
  selectedMonth,
  selectedType,
  selectedYear,
  toggleSearch,
} = useTransactionsView()
</script>

<template>
  <div class="animate-fade-in space-y-10">
    <TransactionsHeader
      :is-search-open="isSearchOpen"
      :search-query="searchQuery"
      @toggle-search="toggleSearch"
      @update:search-query="searchQuery = $event"
    />

    <TransactionsFilters
      :selected-month="selectedMonth"
      :selected-year="selectedYear"
      :selected-type="selectedType"
      @update:selected-month="selectedMonth = $event"
      @update:selected-year="selectedYear = $event"
      @select-type="selectType"
    />

    <LoadingIndicator :is-loading="isLoading" message="Sincronizando Historial..." />

    <section v-if="!isLoading && groupedTransactions.length > 0" class="space-y-12">
      <TransactionsList :groups="groupedTransactions" :format-currency="formatCurrency" @select-transaction="navigateToEdit" />
      <TransactionsInsightCard />
    </section>

    <TransactionsEmptyState v-else-if="!isLoading" />
  </div>
</template>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.8s ease-out forwards;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>

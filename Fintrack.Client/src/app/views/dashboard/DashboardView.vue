<script setup lang="ts">
import AppButton from '@/app/components/common/AppButton.vue'
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import DashboardBalanceSection from '@/app/components/dashboard/DashboardBalanceSection.vue'
import DashboardBudgetsSection from '@/app/components/dashboard/DashboardBudgetsSection.vue'
import DashboardRecentActivitySection from '@/app/components/dashboard/DashboardRecentActivitySection.vue'
import { useDashboardSummary } from './composables/useDashboardSummary'

const { dashboardData, isLoading, hasError, fetchDashboardData } = useDashboardSummary()
</script>

<template>
  <div>
    <LoadingIndicator :is-loading="isLoading" message="Cargando datos del tablero..." />

    <div v-if="dashboardData && !isLoading" class="space-y-8">
      <DashboardBalanceSection
        :total-balance="dashboardData.totalBalance"
        :monthly-income="dashboardData.monthlyIncome"
        :monthly-expenses="dashboardData.monthlyExpenses"
      />

      <DashboardBudgetsSection :budgets="dashboardData.topBudgets" />

      <DashboardRecentActivitySection :transactions="dashboardData.recentTransactions" />
    </div>

    <div v-else-if="hasError && !isLoading" class="flex flex-col items-center justify-center min-h-[400px] gap-4">
      <span class="material-symbols-outlined text-[#FF4D4D] text-5xl">error</span>
      <h4 class="text-lg font-bold text-[#F3F4F6]">Error al cargar datos</h4>
      <p class="text-[#bacbbe] text-sm text-center">Por favor intenta recargar la página más tarde.</p>
      <AppButton
        type="button"
        variant="secondary"
        :full-width="false"
        class="!w-auto !px-6 !py-3"
        @click="fetchDashboardData"
      >
        Reintentar
      </AppButton>
    </div>
  </div>
</template>

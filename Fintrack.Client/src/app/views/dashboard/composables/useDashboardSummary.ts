import { onMounted, ref } from 'vue'
import dashboardService, { type DashboardSummaryDto } from '@/services/dashboardService'

const padTimePart = (value: number): string => value.toString().padStart(2, '0')

const getCurrentLocalIsoWithOffset = (): string => {
  const date = new Date()
  const offset = -date.getTimezoneOffset()
  const sign = offset >= 0 ? '+' : '-'
  const absOffset = Math.abs(offset)
  const offsetHours = padTimePart(Math.floor(absOffset / 60))
  const offsetMinutes = padTimePart(absOffset % 60)

  return `${date.getFullYear()}-${padTimePart(date.getMonth() + 1)}-${padTimePart(date.getDate())}T${padTimePart(date.getHours())}:${padTimePart(date.getMinutes())}:${padTimePart(date.getSeconds())}${sign}${offsetHours}:${offsetMinutes}`
}

export const useDashboardSummary = () => {
  const dashboardData = ref<DashboardSummaryDto | null>(null)
  const isLoading = ref(true)
  const hasError = ref(false)

  const fetchDashboardData = async (): Promise<void> => {
    try {
      isLoading.value = true
      hasError.value = false
      dashboardData.value = await dashboardService.getDashboardSummary(getCurrentLocalIsoWithOffset())
    } catch (error) {
      hasError.value = true
      console.error('Failed to fetch dashboard data:', error)
    } finally {
      isLoading.value = false
    }
  }

  onMounted(fetchDashboardData)

  return {
    dashboardData,
    hasError,
    isLoading,
    fetchDashboardData,
  }
}

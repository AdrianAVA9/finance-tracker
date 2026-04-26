import type { MonthlyExpenseSummaryDto } from '@/services/budgetService'
import { parseCalendarDateFromApi } from '@/app/utils/calendarDate'

export const formatBudgetCurrency = (value: number): string =>
  new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(value)

export const formatBudgetExpenseDate = (dateString: string): string => {
  const date = parseCalendarDateFromApi(dateString)
  return date.toLocaleDateString('es-ES', { month: 'short', day: '2-digit', year: 'numeric' })
}

export const getMonthShortLabel = (month: number): string => {
  const date = new Date()
  date.setMonth(month - 1)
  return date.toLocaleString('es-ES', { month: 'short' })
}

export const chartDataChronological = (
  monthlyHistory: MonthlyExpenseSummaryDto[] | undefined,
): MonthlyExpenseSummaryDto[] => {
  if (!monthlyHistory?.length) return []
  return [...monthlyHistory].reverse()
}

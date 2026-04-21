import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import budgetService, { type BudgetDetailsDto } from '@/services/budgetService'
import { chartDataChronological } from '@/app/components/budget/budgetDetailsFormatters'

export const useBudgetDetails = () => {
  const route = useRoute()
  const budgetData = ref<BudgetDetailsDto | null>(null)
  const isLoading = ref(true)
  const openMonthIndex = ref<number | null>(null)

  const resolveBudgetId = (): string | null => {
    const raw = route.params.id
    if (typeof raw === 'string' && raw.length > 0) return raw
    if (Array.isArray(raw) && raw[0]) return raw[0]
    return null
  }

  const fetchDetails = async (): Promise<void> => {
    const id = resolveBudgetId()
    if (!id) {
      isLoading.value = false
      budgetData.value = null
      return
    }

    try {
      isLoading.value = true
      const now = new Date()
      budgetData.value = await budgetService.getBudgetDetails(id, now.getMonth() + 1, now.getFullYear())
    } catch (error) {
      console.error('Failed to fetch budget details:', error)
      budgetData.value = null
    } finally {
      isLoading.value = false
    }
  }

  const chartData = computed(() => chartDataChronological(budgetData.value?.monthlyHistory))

  const maxExpense = computed(() => {
    const history = budgetData.value?.monthlyHistory
    const limit = budgetData.value?.limitAmount ?? 0
    if (!history?.length) return limit
    const maxVal = Math.max(...history.map((m) => m.totalExpense || 0))
    return Math.max(maxVal, limit)
  })

  const totalSpentYTD = computed(() => {
    const history = budgetData.value?.monthlyHistory
    if (!history?.length) return 0
    const currentYear = new Date().getFullYear()
    return history.filter((m) => m.year === currentYear).reduce((sum, m) => sum + (m.totalExpense || 0), 0)
  })

  const toggleMonth = (index: number): void => {
    openMonthIndex.value = openMonthIndex.value === index ? null : index
  }

  onMounted(() => {
    void fetchDetails()
  })

  return {
    budgetData,
    chartData,
    fetchDetails,
    isLoading,
    maxExpense,
    openMonthIndex,
    toggleMonth,
    totalSpentYTD,
  }
}

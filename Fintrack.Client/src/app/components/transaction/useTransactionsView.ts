import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { parseCalendarDateFromApi } from '@/app/utils/calendarDate'
import transactionService, { type TransactionDto } from '@/services/transactionService'

export function useTransactionsView() {
  const router = useRouter()

  const transactions = ref<TransactionDto[]>([])
  const isLoading = ref(true)
  const selectedYear = ref(new Date().getFullYear())
  const selectedMonth = ref(new Date().getMonth() + 1)
  const selectedType = ref<'All' | 'Income' | 'Expense'>('All')
  const isSearchOpen = ref(false)
  const searchQuery = ref('')

  const normalize = (value: string): string =>
    value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase()

  const fetchTransactions = async () => {
    try {
      isLoading.value = true
      transactions.value = await transactionService.getTransactions(
        selectedYear.value,
        selectedMonth.value,
        selectedType.value,
      )
    } catch (error) {
      console.error('Failed to fetch transactions', error)
    } finally {
      isLoading.value = false
    }
  }

  const filteredTransactions = computed(() => {
    const query = searchQuery.value.trim()
    if (!query) return transactions.value

    const normalizedQuery = normalize(query)

    return transactions.value.filter((transaction) => {
      return (
        normalize(transaction.description).includes(normalizedQuery) ||
        normalize(transaction.categoryName).includes(normalizedQuery)
      )
    })
  })

  const groupedTransactions = computed(() => {
    const groups: Record<string, TransactionDto[]> = {}
    const locale = navigator.language || 'es-ES'

    const today = new Date()
    today.setHours(0, 0, 0, 0)

    const yesterday = new Date(today)
    yesterday.setDate(today.getDate() - 1)

    filteredTransactions.value.forEach((transaction) => {
      const date = parseCalendarDateFromApi(transaction.date)

      let groupKey = ''
      if (date.toDateString() === today.toDateString()) {
        groupKey = 'Hoy'
      } else if (date.toDateString() === yesterday.toDateString()) {
        groupKey = 'Ayer'
      } else {
        groupKey = date.toLocaleDateString(locale, { weekday: 'long', day: 'numeric', month: 'long' })
        groupKey = groupKey.charAt(0).toUpperCase() + groupKey.slice(1)
      }

      if (!groups[groupKey]) {
        groups[groupKey] = []
      }
      groups[groupKey]!.push(transaction)
    })

    return Object.entries(groups).map(([date, items]) => ({ date, items }))
  })

  const formatCurrency = (amount: number) =>
    new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC' }).format(amount)

  const navigateToEdit = (transaction: TransactionDto) => {
    const routeName = transaction.type === 'Income' ? 'IncomeEdit' : 'ExpenseEdit'
    const id = transaction.id.includes('_') ? transaction.id.split('_')[1] : transaction.id
    router.push({ name: routeName, params: { id } })
  }

  const toggleSearch = () => {
    isSearchOpen.value = !isSearchOpen.value
    if (isSearchOpen.value) return
    searchQuery.value = ''
  }

  const selectType = (type: 'All' | 'Income' | 'Expense') => {
    selectedType.value = type
  }

  onMounted(fetchTransactions)
  watch([selectedYear, selectedMonth, selectedType], fetchTransactions)

  return {
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
    formatCurrency,
  }
}

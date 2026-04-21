import { ref, computed, onMounted } from 'vue'
import api from '@/services/api'

export interface ExpenseFormItem {
  id: string
  categoryId: number | null
  itemAmount: number | null
  description: string
}

export interface ExpenseCategoryDto {
  id: number
  name: string
  group?: { name: string }
}

export interface ExpenseInitialPayload {
  merchant: string
  totalAmount: number
  date: string
  items?: Array<{ categoryId: number; itemAmount: number; description: string }>
}

export interface ExpenseSubmitPayload {
  merchant: string
  totalAmount: number
  date: string
  items: Array<{ categoryId: number | null; itemAmount: number; description: string }>
}

function createEmptySplitRows(): ExpenseFormItem[] {
  return [
    { id: crypto.randomUUID(), categoryId: null, itemAmount: null, description: '' },
    { id: crypto.randomUUID(), categoryId: null, itemAmount: null, description: '' }
  ]
}

export function useExpenseRegistrationForm(getInitialData: () => ExpenseInitialPayload | null | undefined) {
  const merchant = ref('')
  const totalAmount = ref<number | null>(null)
  const date = ref(new Date().toLocaleDateString('en-CA'))

  const singleCategoryId = ref<number | null>(null)

  const isSplitInvoice = ref(false)
  const items = ref<ExpenseFormItem[]>(createEmptySplitRows())

  const categories = ref<ExpenseCategoryDto[]>([])

  const loadCategories = async () => {
    try {
      const { data } = await api.get<ExpenseCategoryDto[]>('/api/v1/expensecategories')
      if (!Array.isArray(data)) {
        console.warn('API returned non-array data:', data)
        return
      }
      categories.value = data
    } catch (error) {
      console.error('Failed to load categories', error)
    }
  }

  const runningSum = computed(() => {
    if (!isSplitInvoice.value) return totalAmount.value || 0
    return items.value.reduce((sum, item) => sum + (Number(item.itemAmount) || 0), 0)
  })

  const isMathMismatch = computed(() => {
    if (!isSplitInvoice.value) return false
    const total = Number(totalAmount.value) || 0
    return Math.abs(runningSum.value - total) > 0.001
  })

  const missingAmount = computed(() => {
    const total = Number(totalAmount.value) || 0
    return total - runningSum.value
  })

  const isSubmitDisabled = computed(() => {
    if (!totalAmount.value || totalAmount.value <= 0) return true
    if (!merchant.value) return true
    if (!date.value) return true

    if (isSplitInvoice.value) {
      if (isMathMismatch.value) return true
      if (items.value.some((i) => !i.categoryId || !i.itemAmount || i.itemAmount <= 0)) return true
    } else {
      if (!singleCategoryId.value) return true
    }
    return false
  })

  const toggleSplitMode = () => {
    isSplitInvoice.value = !isSplitInvoice.value
    if (!isSplitInvoice.value) {
      items.value = []
    } else {
      items.value = createEmptySplitRows()
    }
  }

  const addRow = () => {
    items.value.push({
      id: crypto.randomUUID(),
      categoryId: null,
      itemAmount: null,
      description: ''
    })
  }

  const removeRow = (id: string) => {
    items.value = items.value.filter((i) => i.id !== id)
  }

  function hydrateFromInitial(data: ExpenseInitialPayload) {
    merchant.value = data.merchant
    totalAmount.value = data.totalAmount
    date.value = data.date.split('T')[0]

    if (data.items && data.items.length > 0) {
      if (data.items.length > 1) {
        isSplitInvoice.value = true
        items.value = data.items.map((item) => ({
          id: crypto.randomUUID(),
          categoryId: item.categoryId,
          itemAmount: item.itemAmount,
          description: item.description
        }))
      } else {
        isSplitInvoice.value = false
        const first = data.items[0]
        if (first) singleCategoryId.value = first.categoryId
      }
    }
  }

  onMounted(async () => {
    await loadCategories()
    const initial = getInitialData()
    if (initial) hydrateFromInitial(initial)
  })

  function buildSubmitPayload(): ExpenseSubmitPayload {
    return {
      merchant: merchant.value,
      totalAmount: Number(totalAmount.value),
      date: date.value,
      items: isSplitInvoice.value
        ? items.value.map((i) => ({
            categoryId: i.categoryId,
            itemAmount: Number(i.itemAmount),
            description: i.description
          }))
        : [
            {
              categoryId: singleCategoryId.value,
              itemAmount: Number(totalAmount.value),
              description: merchant.value
            }
          ]
    }
  }

  return {
    merchant,
    totalAmount,
    date,
    singleCategoryId,
    isSplitInvoice,
    items,
    categories,
    runningSum,
    isMathMismatch,
    missingAmount,
    isSubmitDisabled,
    toggleSplitMode,
    addRow,
    removeRow,
    buildSubmitPayload
  }
}

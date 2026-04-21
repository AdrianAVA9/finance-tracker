import { ref, computed, onMounted } from 'vue'
import api from '@/services/api'

export interface IncomeCategoryDto {
  id: string
  name: string
  icon?: string | null
  color?: string | null
  isEditable?: boolean
}

/** Matches API `IncomeDetailsDto` (camelCase JSON). */
export interface IncomeDetailsDto {
  id: string
  source: string
  amount: number
  categoryId: string
  date: string
  notes: string | null
  isRecurring: boolean
  frequency: string | null
  nextDate: string | null
}

export interface IncomeSubmitPayload {
  source: string
  amount: number
  categoryId: string
  date: string
  notes: string | null
  isRecurring: boolean
  frequency: string | null
  nextDate: string | null
}

export const INCOME_FREQUENCY_OPTIONS = [
  { value: 'Daily', label: 'Diario' },
  { value: 'Weekly', label: 'Semanal' },
  { value: 'BiWeekly', label: 'Quincenal' },
  { value: 'Monthly', label: 'Mensual' },
  { value: 'Quarterly', label: 'Trimestral' },
  { value: 'Yearly', label: 'Anual' }
] as const

export function useIncomeRegistrationForm(getInitialData: () => IncomeDetailsDto | null | undefined) {
  const amount = ref<number | null>(null)
  const source = ref('')
  const date = ref(new Date().toLocaleDateString('en-CA'))
  const categoryId = ref<string | null>(null)
  const notes = ref('')
  const isRecurring = ref(false)
  const frequency = ref<string>('Monthly')
  const nextDate = ref('')

  const categories = ref<IncomeCategoryDto[]>([])

  const loadCategories = async () => {
    const { data } = await api.get<IncomeCategoryDto[]>('/api/v1/incomes/categories')
    categories.value = Array.isArray(data) ? data : []
    const initial = getInitialData()
    if (!initial && categories.value.length > 0 && categories.value[0]) {
      categoryId.value = categories.value[0].id
    }
  }

  function hydrateFromDto(data: IncomeDetailsDto) {
    amount.value = Number(data.amount)
    source.value = data.source
    date.value = typeof data.date === 'string' ? (data.date.split('T')[0] || '') : ''
    categoryId.value = data.categoryId
    notes.value = data.notes ?? ''
    isRecurring.value = data.isRecurring
    frequency.value = data.frequency || 'Monthly'
    nextDate.value = data.nextDate ? (data.nextDate.split('T')[0] || '') : ''
  }

  function resetForNewEntry() {
    amount.value = null
    source.value = ''
    date.value = new Date().toLocaleDateString('en-CA')
    notes.value = ''
    isRecurring.value = false
    frequency.value = 'Monthly'
    nextDate.value = ''
    if (categories.value.length > 0 && categories.value[0]) {
      categoryId.value = categories.value[0].id
    } else {
      categoryId.value = null
    }
  }

  function buildPayload(): IncomeSubmitPayload {
    return {
      source: source.value,
      amount: Number(amount.value),
      categoryId: categoryId.value!,
      date: date.value,
      notes: notes.value.trim() === '' ? null : notes.value,
      isRecurring: isRecurring.value,
      frequency: isRecurring.value ? frequency.value : null,
      nextDate: isRecurring.value && nextDate.value ? nextDate.value : null
    }
  }

  const isSubmitDisabled = computed(() => {
    if (amount.value === null || amount.value <= 0) return true
    if (!source.value.trim()) return true
    if (!categoryId.value) return true
    if (!date.value) return true
    if (isRecurring.value && !nextDate.value) return true
    return false
  })

  const selectedCategory = computed(() => categories.value.find(c => c.id === categoryId.value))

  onMounted(async () => {
    await loadCategories()
    const initial = getInitialData()
    if (initial) hydrateFromDto(initial)
  })

  return {
    amount,
    source,
    date,
    categoryId,
    notes,
    isRecurring,
    frequency,
    nextDate,
    categories,
    selectedCategory,
    isSubmitDisabled,
    buildPayload,
    resetForNewEntry
  }
}

import { ref, computed, watch, onMounted, type Ref } from 'vue'
import api from '@/services/api'

export type UserOwnedCategoryKind = 'income' | 'expense'

export interface UserOwnedIncomeCategory {
  id: string
  name: string
  icon?: string | null
  color?: string | null
  isEditable?: boolean
}

export interface UserOwnedExpenseCategory {
  id: string
  name: string
  color?: string | null
  icon?: string | null
  description?: string | null
  isEditable?: boolean
  group?: { name?: string | null } | null
}

export interface ExpenseGroupSection {
  groupLabel: string
  items: UserOwnedExpenseCategory[]
}

const incomeOwnedUrl = '/api/v1/incomes/categories/owned'
const expenseOwnedUrl = '/api/v1/expensecategories/owned'

function normalizeId(raw: { id: unknown }): string {
  if (typeof raw.id === 'string') return raw.id
  if (typeof raw.id === 'number' || typeof raw.id === 'bigint') return String(raw.id)
  return String(raw.id)
}

function mapIncomeRows(data: unknown): UserOwnedIncomeCategory[] {
  if (!Array.isArray(data)) return []
  return data.map((row) => {
    const o = row as Record<string, unknown>
    return {
      id: normalizeId({ id: o['id'] }),
      name: typeof o['name'] === 'string' ? o['name'] : '',
      icon: o['icon'] as string | null | undefined,
      color: o['color'] as string | null | undefined,
      isEditable: o['isEditable'] as boolean | undefined
    }
  })
}

function mapExpenseRows(data: unknown): UserOwnedExpenseCategory[] {
  if (!Array.isArray(data)) return []
  return data.map((row) => {
    const o = row as Record<string, unknown>
    const groupRaw = o['group'] as Record<string, unknown> | null | undefined
    return {
      id: normalizeId({ id: o['id'] }),
      name: typeof o['name'] === 'string' ? o['name'] : '',
      color: o['color'] as string | null | undefined,
      icon: o['icon'] as string | null | undefined,
      description: o['description'] as string | null | undefined,
      isEditable: o['isEditable'] as boolean | undefined,
      group: groupRaw
        ? { name: typeof groupRaw['name'] === 'string' ? groupRaw['name'] : null }
        : null
    }
  })
}

function groupExpensesByGroupName(items: UserOwnedExpenseCategory[]): ExpenseGroupSection[] {
  const byGroup = new Map<string, UserOwnedExpenseCategory[]>()
  for (const item of items) {
    const key = (item.group?.name && item.group.name.trim() !== '' ? item.group.name : 'Sin grupo') as string
    const list = byGroup.get(key)
    if (list) {
      list.push(item)
    } else {
      byGroup.set(key, [item])
    }
  }
  return [...byGroup.entries()]
    .sort(([a], [b]) => a.localeCompare(b, 'es', { sensitivity: 'base' }))
    .map(([groupLabel, sectionItems]) => ({ groupLabel, items: sectionItems }))
}

export function useUserOwnedCategoryList(kind: Ref<UserOwnedCategoryKind>) {
  const isLoading = ref(true)
  const loadError = ref<string | null>(null)
  const incomeItems = ref<UserOwnedIncomeCategory[]>([])
  const expenseItems = ref<UserOwnedExpenseCategory[]>([])

  const search = ref('')

  const filterIncome = (items: UserOwnedIncomeCategory[], q: string) => {
    if (!q.trim()) return items
    const n = q.trim().toLowerCase()
    return items.filter((c) => c.name.toLowerCase().includes(n))
  }

  const filterExpense = (items: UserOwnedExpenseCategory[], q: string) => {
    if (!q.trim()) return items
    const n = q.trim().toLowerCase()
    return items.filter(
      (c) =>
        c.name.toLowerCase().includes(n) ||
        (c.group?.name?.toLowerCase().includes(n) ?? false)
    )
  }

  const visibleIncome = computed(() => filterIncome(incomeItems.value, search.value))

  const visibleExpense = computed(() => filterExpense(expenseItems.value, search.value))

  const expenseSections = computed(() => groupExpensesByGroupName(visibleExpense.value))

  async function load() {
    isLoading.value = true
    loadError.value = null
    try {
      const k = kind.value
      if (k === 'income') {
        const { data } = await api.get<unknown>(incomeOwnedUrl)
        incomeItems.value = mapIncomeRows(data)
        expenseItems.value = []
      } else {
        const { data } = await api.get<unknown>(expenseOwnedUrl)
        expenseItems.value = mapExpenseRows(data)
        incomeItems.value = []
      }
    } catch {
      loadError.value = 'No se pudieron cargar las categorías. Intenta de nuevo.'
    } finally {
      isLoading.value = false
    }
  }

  onMounted(() => {
    void load()
  })

  watch(kind, () => {
    void load()
  })

  return {
    isLoading,
    loadError,
    search,
    incomeItems,
    expenseItems,
    visibleIncome,
    expenseSections,
    load
  }
}

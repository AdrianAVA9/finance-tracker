import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import BudgetDetailsView from './BudgetDetailsView.vue'

const { mockGetBudgetDetails } = vi.hoisted(() => ({
  mockGetBudgetDetails: vi.fn(),
}))

vi.mock('vue-router', () => ({
  useRoute: () => ({
    params: { id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa' },
  }),
}))

vi.mock('@/services/budgetService', () => ({
  default: {
    getBudgetDetails: mockGetBudgetDetails,
  },
}))

describe('BudgetDetailsView.vue', () => {
  const mockDetails = {
    id: 1,
    categoryName: 'Rent',
    limitAmount: 1000,
    monthlyHistory: [
      {
        month: 1,
        year: 2026,
        totalExpense: 100,
        expenses: [{ id: 1, description: 'Coffee', amount: 50, date: '2026-01-10' }],
      },
    ],
  }

  beforeEach(() => {
    vi.clearAllMocks()
    mockGetBudgetDetails.mockResolvedValue(mockDetails)
  })

  it('loads details with string route id and shows category', async () => {
    const wrapper = mount(BudgetDetailsView)
    await flushPromises()

    expect(mockGetBudgetDetails).toHaveBeenCalledWith(
      'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
      expect.any(Number),
      expect.any(Number),
    )
    expect(wrapper.text()).toContain('Rent')
    expect(wrapper.text()).toContain('Total Gastado Este Año')
  })

  it('shows empty error state when fetch fails', async () => {
    mockGetBudgetDetails.mockRejectedValueOnce(new Error('network'))
    const wrapper = mount(BudgetDetailsView)
    await flushPromises()

    expect(wrapper.text()).toContain('Presupuesto no encontrado')
  })
})

import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import BudgetBulkEditView from './BudgetBulkEditView.vue'

const { mockPush, mockGetBudgets, mockUpsertBatch } = vi.hoisted(() => ({
  mockPush: vi.fn(),
  mockGetBudgets: vi.fn(),
  mockUpsertBatch: vi.fn(),
}))

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}))

vi.mock('@/services/budgetService', () => ({
  default: {
    getBudgets: mockGetBudgets,
    upsertBatch: mockUpsertBatch,
  },
}))

describe('BudgetBulkEditView.vue', () => {
  const mockBudgets = [
    {
      id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
      categoryId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
      categoryName: 'Rent',
      limitAmount: 1000,
      spentAmount: 800,
      categoryColor: '#F00',
      categoryIcon: 'home',
      isRecurrent: true,
    },
    {
      id: 'cccccccc-cccc-cccc-cccc-cccccccccccc',
      categoryId: 'dddddddd-dddd-dddd-dddd-dddddddddddd',
      categoryName: 'Food',
      limitAmount: 500,
      spentAmount: 100,
      categoryColor: '#0F0',
      categoryIcon: 'restaurant',
      isRecurrent: false,
    },
  ]

  const mockResponse = {
    budgets: mockBudgets,
    monthlyIncome: 5000,
  }

  beforeEach(() => {
    vi.clearAllMocks()
    mockGetBudgets.mockResolvedValue(mockResponse)
    mockUpsertBatch.mockResolvedValue(undefined)
  })

  it('loads budgets on mount and shows category count', async () => {
    const month = new Date().getMonth() + 1
    const year = new Date().getFullYear()

    const wrapper = mount(BudgetBulkEditView)
    await flushPromises()

    expect(mockGetBudgets).toHaveBeenCalledTimes(1)
    expect(mockGetBudgets).toHaveBeenCalledWith(month, year)
    expect(wrapper.text()).toContain('Edición Múltiple • 2 categorías')
    expect(wrapper.text()).toContain('Rent')
    expect(wrapper.text()).toContain('Food')
  })

  it('shows baseline and simulated totals matching loaded limits', async () => {
    const wrapper = mount(BudgetBulkEditView)
    await flushPromises()

    expect(wrapper.text()).toContain('Total Base')
    expect(wrapper.text()).toContain('Simulado')
    expect(wrapper.text()).toMatch(/1\s?500/)
  })

  it('discard resets draft values to server baseline', async () => {
    const wrapper = mount(BudgetBulkEditView)
    await flushPromises()

    const inputs = wrapper.findAll('input[type="text"]')
    expect(inputs.length).toBeGreaterThanOrEqual(2)

    await inputs[0]!.setValue('2000')
    await inputs[0]!.trigger('input')
    await flushPromises()

    expect(wrapper.text()).toMatch(/2\s?500/)

    const deshacer = wrapper.findAll('button').find((b) => b.text().includes('Deshacer'))
    expect(deshacer).toBeDefined()
    await deshacer!.trigger('click')
    await flushPromises()

    expect(wrapper.text()).not.toMatch(/2\s?500/)
  })

  it('calls upsertBatch and navigates to BudgetList on save', async () => {
    const now = new Date()
    const month = now.getMonth() + 1
    const year = now.getFullYear()

    const wrapper = mount(BudgetBulkEditView)
    await flushPromises()

    const buttons = wrapper.findAll('button')
    const saveButton = buttons.find((b) => b.text().includes('Guardar'))
    expect(saveButton).toBeDefined()

    await saveButton!.trigger('click')
    await flushPromises()

    expect(mockUpsertBatch).toHaveBeenCalledWith({
      month,
      year,
      budgets: expect.arrayContaining([
        expect.objectContaining({
          categoryId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
          amount: 1000,
          isRecurrent: true,
        }),
        expect.objectContaining({
          categoryId: 'dddddddd-dddd-dddd-dddd-dddddddddddd',
          amount: 500,
          isRecurrent: false,
        }),
      ]),
    })
    expect(mockPush).toHaveBeenCalledWith({ name: 'BudgetList' })
  })
})

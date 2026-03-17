import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import BudgetRegistrationView from './BudgetRegistrationView.vue'
import api from '@/services/api'

// Mock the API
vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn()
  }
}))

describe('BudgetRegistrationView.vue', () => {
  const mockBudgets = [
    { id: 1, categoryId: 1, categoryName: 'Rent', limitAmount: 1000, spentAmount: 800, categoryColor: '#F00', categoryIcon: 'home' },
    { id: 2, categoryId: 2, categoryName: 'Food', limitAmount: 500, spentAmount: 100, categoryColor: '#0F0', categoryIcon: 'restaurant' }
  ]

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders stats correctly based on loaded budgets', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockBudgets })
    
    const wrapper = mount(BudgetRegistrationView)
    await flushPromises()

    // Total Budgeted should be 1000 + 500 = 1500
    // Total Spent should be 800 + 100 = 900
    const text = wrapper.text()
    expect(text).toContain('₡1,500') // Total Budgeted
    expect(text).toContain('₡900')   // Total Spent
  })

  it('opens CategoryBudgetModal when "Add Budget" is clicked', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: [] })
    const wrapper = mount(BudgetRegistrationView)
    await flushPromises()

    const addBtn = wrapper.findAll('button').find(b => b.text().includes('Agregar presupuesto'))
    expect(addBtn).toBeTruthy()
    
    await addBtn?.trigger('click')
    
    // Check if modal is visible. The component is imported as CategoryBudgetModal
    // We can check if the modal text is present in the wrapper
    expect(wrapper.text()).toContain('Configurar Presupuesto')
  })

  it('calculates "Remaining to Allocate" correctly', async () => {
    // Assuming expected income is static or from another source for now.
    // In my implementation, I used a static 5000 for income if not provided.
    vi.mocked(api.get).mockResolvedValue({ data: mockBudgets })
    const wrapper = mount(BudgetRegistrationView)
    await flushPromises()

    // Income (5000) - Budgeted (1500) = 3500
    expect(wrapper.text()).toContain('₡3,500')
  })

  it('filters budgets by search term', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockBudgets })
    const wrapper = mount(BudgetRegistrationView)
    await flushPromises()

    const searchInput = wrapper.find('input[placeholder*="Buscar"]')
    await searchInput.setValue('Rent')

    expect(wrapper.text()).toContain('Rent')
    expect(wrapper.text()).not.toContain('Food')
  })
})

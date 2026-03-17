import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi } from 'vitest'
import CategoryBudgetModal from './CategoryBudgetModal.vue'
import api from '@/services/api'

// Mock the API
vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn()
  }
}))

// Mock crypto.randomUUID as it might not be available in all jsdom environments
if (typeof window !== 'undefined' && !window.crypto) {
  (window as any).crypto = { randomUUID: () => 'test-uuid' }
} else if (typeof window !== 'undefined' && window.crypto && !window.crypto.randomUUID) {
  (window.crypto as any).randomUUID = () => 'test-uuid'
}

describe('CategoryBudgetModal.vue', () => {
  const mockCategories = [
    { id: 1, name: 'Food', color: '#FF5733', icon: 'restaurant' },
    { id: 2, name: 'Transport', color: '#3357FF', icon: 'directions_bus' }
  ]

  it('loads categories on mount', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    
    const wrapper = mount(CategoryBudgetModal, {
      props: {
        isOpen: true,
        month: 3,
        year: 2024
      }
    })

    await flushPromises()

    const options = wrapper.findAll('option')
    expect(options.length).toBeGreaterThan(mockCategories.length)
    expect(wrapper.text()).toContain('Food')
    expect(wrapper.text()).toContain('Transport')
  })

  it('disables save button if amount is not set', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    const wrapper = mount(CategoryBudgetModal, {
      props: { isOpen: true, month: 3, year: 2024 }
    })
    await flushPromises()

    const saveBtn = wrapper.findAll('button').find(b => b.text().includes('Guardar Presupuesto'))
    expect((saveBtn?.element as HTMLButtonElement).disabled).toBe(true)

    await wrapper.find('select').setValue(1)
    expect((saveBtn?.element as HTMLButtonElement).disabled).toBe(true)
  })

  it('emits close and success events on successful save', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    vi.mocked(api.post).mockResolvedValue({ data: { id: 100 } })

    const wrapper = mount(CategoryBudgetModal, {
      props: { isOpen: true, month: 3, year: 2024 }
    })
    await flushPromises()

    await wrapper.find('select').setValue(1)
    await wrapper.find('input[type="number"]').setValue(1200)

    const saveBtn = wrapper.findAll('button').find(b => b.text().includes('Guardar Presupuesto'))
    await saveBtn?.trigger('click')
    await flushPromises()

    expect(api.post).toHaveBeenCalledWith('/api/v1/budgets/batch', expect.objectContaining({
      budgets: [{ categoryId: 1, amount: 1200 }]
    }))
    
    expect(wrapper.emitted('saved')).toBeTruthy()
    expect(wrapper.emitted('close')).toBeTruthy()
  })

  it('populates fields when editing an existing budget', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    
    const existingBudget = {
      id: 50,
      categoryId: 2,
      limitAmount: 450,
      categoryName: 'Transport'
    }

    const wrapper = mount(CategoryBudgetModal, {
      props: {
        isOpen: true,
        month: 3,
        year: 2024,
        budget: existingBudget
      }
    })
    await flushPromises()

    expect((wrapper.find('select').element as HTMLSelectElement).value).toBe('2')
    expect((wrapper.find('input[type="number"]').element as HTMLInputElement).value).toBe('450')
    expect(wrapper.text()).toContain('Editar Presupuesto')
  })
})

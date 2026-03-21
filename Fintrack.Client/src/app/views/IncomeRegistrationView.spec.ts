import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import IncomeRegistrationView from './IncomeRegistrationView.vue'
import api from '@/services/api'

// Mock the API
vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn()
  }
}))

// Mock router
const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush
  })
}))

describe('IncomeRegistrationView.vue', () => {
  const mockCategories = [
    { id: 1, name: 'Salario', icon: '💼', color: '#10B981' },
    { id: 2, name: 'Inversión', icon: '📈', color: '#06B6D4' }
  ]

  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    vi.mocked(api.post).mockResolvedValue({ data: { id: 123 } })
  })

  it('loads and displays categories on mount', async () => {
    const wrapper = mount(IncomeRegistrationView)
    await flushPromises()

    expect(api.get).toHaveBeenCalledWith('/api/v1/incomes/categories')
    const options = wrapper.findAll('option')
    expect(options.length).toBe(2)
    expect(options[0]?.text()).toContain('Salario')
  })

  it('shows recurrence fields only when toggle is on', async () => {
    const wrapper = mount(IncomeRegistrationView)
    await flushPromises()

    // Initially hidden
    expect(wrapper.find('[data-purpose="expanded-fields"]').exists()).toBe(false)
    // Actually in my implementation I used v-if on a div that contains "Frecuencia"
    expect(wrapper.text()).not.toContain('Frecuencia')

    const toggle = wrapper.find('button.relative.inline-flex')
    await toggle.trigger('click')

    expect(wrapper.text()).toContain('Frecuencia')
  })

  it('keeps data in recurrence fields if toggled off and on', async () => {
    const wrapper = mount(IncomeRegistrationView)
    await flushPromises()

    // Toggle ON
    await wrapper.find('button.relative.inline-flex').trigger('click')
    
    const freqSelect = wrapper.find('select.text-sm') // The frequency select
    await freqSelect.setValue('Weekly')

    // Toggle OFF
    await wrapper.find('button.relative.inline-flex').trigger('click')
    expect(wrapper.text()).not.toContain('Frecuencia')

    // Toggle ON again
    await wrapper.find('button.relative.inline-flex').trigger('click')
    
    const freqSelectNew = wrapper.find('select.text-sm')
    expect((freqSelectNew.element as HTMLSelectElement).value).toBe('Weekly')
  })

  it('submits correctly and redirects on success', async () => {
    const wrapper = mount(IncomeRegistrationView)
    await flushPromises()

    await wrapper.find('input[type="number"]').setValue(1500)
    await wrapper.find('input[placeholder*="Ej. Salario"]').setValue('Salario Marzo')
    
    // Select first category is automatic since it's the first in mock

    const submitBtn = wrapper.find('button[type="submit"]')
    await submitBtn.trigger('submit') // Form @submit.prevent handles it

    expect(api.post).toHaveBeenCalledWith('/api/v1/incomes', expect.objectContaining({
      amount: 1500,
      source: 'Salario Marzo'
    }))

    await flushPromises()
    expect(mockPush).toHaveBeenCalledWith('/app/dashboard')
  })

  it('resets form when "Guardar y registrar otro" is clicked', async () => {
    const wrapper = mount(IncomeRegistrationView)
    await flushPromises()

    await wrapper.find('input[type="number"]').setValue(1500)
    await wrapper.find('input[placeholder*="Ej. Salario"]').setValue('Salario Marzo')

    const resetBtn = wrapper.findAll('button').find(b => b.text().includes('Guardar y registrar otro'))
    await resetBtn?.trigger('click')

    await flushPromises()
    
    // Amount should be null (empty)
    expect((wrapper.find('input[type="number"]').element as HTMLInputElement).value).toBe('')
    // Source should be empty
    expect((wrapper.find('input[placeholder*="Ej. Salario"]').element as HTMLInputElement).value).toBe('')
    
    expect(api.post).toHaveBeenCalled()
    expect(mockPush).not.toHaveBeenCalled()
  })
})

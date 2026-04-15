import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import IncomeRegistrationForm from './IncomeRegistrationForm.vue'
import CategorySelector from '@/app/components/common/CategorySelector.vue'
import api from '@/services/api'

vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn()
  }
}))

describe('IncomeRegistrationForm.vue', () => {
  const catA = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee'
  const catB = 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff'

  const mockCategories = [
    { id: catA, name: 'Salario', icon: 'payments', color: '#10B981' },
    { id: catB, name: 'Inversión', icon: 'trending_up', color: '#06B6D4' }
  ]

  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
  })

  it('loads categories on mount', async () => {
    mount(IncomeRegistrationForm)
    await flushPromises()
    expect(api.get).toHaveBeenCalledWith('/api/v1/incomes/categories')
  })

  it('shows recurrence fields when toggle is on', async () => {
    const wrapper = mount(IncomeRegistrationForm)
    await flushPromises()

    expect(wrapper.find('[data-purpose="expanded-fields"]').exists()).toBe(false)

    await wrapper.get('[data-testid="income-recurring-toggle"]').setValue(true)

    expect(wrapper.find('[data-purpose="expanded-fields"]').exists()).toBe(true)
    expect(wrapper.text()).toContain('Frecuencia')
  })

  it('keeps frequency when toggling recurrence off and on', async () => {
    const wrapper = mount(IncomeRegistrationForm)
    await flushPromises()

    await wrapper.get('[data-testid="income-recurring-toggle"]').setValue(true)
    await wrapper.get('[data-testid="income-frequency-select"]').setValue('Weekly')

    await wrapper.get('[data-testid="income-recurring-toggle"]').setValue(false)
    expect(wrapper.find('[data-purpose="expanded-fields"]').exists()).toBe(false)

    await wrapper.get('[data-testid="income-recurring-toggle"]').setValue(true)
    expect((wrapper.get('[data-testid="income-frequency-select"]').element as HTMLSelectElement).value).toBe('Weekly')
  })

  it('emits submit with payload when form is valid', async () => {
    const wrapper = mount(IncomeRegistrationForm)
    await flushPromises()

    await wrapper.get('[data-testid="income-amount-card"] input[type="number"]').setValue(1500)
    await wrapper.get('[data-testid="income-source-input"]').setValue('Salario Marzo')
    await wrapper.get('[data-testid="income-date-input"]').setValue('2025-03-15')
    await wrapper.findComponent(CategorySelector).vm.$emit('update:modelValue', catA)

    await wrapper.find('form').trigger('submit')
    await flushPromises()

    const emitted = wrapper.emitted('submit')
    expect(emitted?.[0]?.[0]).toMatchObject({
      amount: 1500,
      source: 'Salario Marzo',
      categoryId: catA,
      isRecurring: false,
      frequency: null,
      nextDate: null
    })
  })

  it('emits save-and-another with valid payload', async () => {
    const wrapper = mount(IncomeRegistrationForm)
    await flushPromises()

    await wrapper.get('[data-testid="income-amount-card"] input[type="number"]').setValue(500)
    await wrapper.get('[data-testid="income-source-input"]').setValue('Freelance')
    await wrapper.get('[data-testid="income-date-input"]').setValue('2025-01-01')
    await wrapper.findComponent(CategorySelector).vm.$emit('update:modelValue', catA)

    await wrapper.get('[data-testid="income-save-and-another"]').trigger('click')
    await flushPromises()

    expect(wrapper.emitted('save-and-another')).toBeTruthy()
  })
})

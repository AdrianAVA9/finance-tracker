import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi } from 'vitest'
import ExpenseRegistrationForm from './ExpenseRegistrationForm.vue'
import CategorySelector from '@/app/components/common/CategorySelector.vue'
import api from '@/services/api'

vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn()
  }
}))

describe('ExpenseRegistrationForm.vue', () => {
  const mockCategories = [
    { id: 1, name: 'Food', group: { name: 'Essentials' } },
    { id: 2, name: 'Transport', group: { name: 'Essentials' } }
  ]

  it('Should keep save button disabled if simple amount is not provided', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    const wrapper = mount(ExpenseRegistrationForm)
    await flushPromises()

    await wrapper.get('[data-testid="expense-merchant-input"]').setValue('Test Merchant')
    await wrapper.get('[data-testid="expense-simple-date"]').setValue('2025-01-01')
    await wrapper.findComponent(CategorySelector).vm.$emit('update:modelValue', 1)

    const submitBtn = wrapper.get('[data-testid="expense-form-submit"]')
    expect((submitBtn.element as HTMLButtonElement).disabled).toBe(true)
  })

  it('Should emit submit event when valid simple form is submitted', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    const wrapper = mount(ExpenseRegistrationForm)
    await flushPromises()

    await wrapper.get('[data-testid="expense-merchant-input"]').setValue('Test Merchant')
    await wrapper.get('[data-testid="expense-simple-date"]').setValue('2025-01-01')
    await wrapper.get('[data-testid="expense-amount-card"] input[type="number"]').setValue(100)
    await wrapper.findComponent(CategorySelector).vm.$emit('update:modelValue', 1)

    await wrapper.find('form').trigger('submit')

    const emitted = wrapper.emitted('submit')
    expect(emitted).toBeTruthy()
    expect(emitted?.[0]?.[0]).toMatchObject({
      merchant: 'Test Merchant',
      totalAmount: 100,
      items: [{ categoryId: 1, itemAmount: 100 }]
    })
  })

  it('Should_DisableSubmitButton_When_RunningSum_DoesNotMatch_TotalAmount in Itemized Mode', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    const wrapper = mount(ExpenseRegistrationForm)
    await flushPromises()

    await wrapper.get('[data-testid="expense-toggle-split"]').trigger('click')
    await wrapper.get('[data-testid="expense-cancel-split"]').trigger('click')

    await wrapper.get('[data-testid="expense-merchant-input"]').setValue('Test Merchant')
    await wrapper.get('[data-testid="expense-simple-date"]').setValue('2025-01-01')
    await wrapper.get('[data-testid="expense-amount-card"] input[type="number"]').setValue(100)

    await wrapper.get('[data-testid="expense-toggle-split"]').trigger('click')

    const itemAmountInputs = wrapper.findAll('input[type="number"][step="0.01"]')
    await itemAmountInputs[0]!.setValue(50.0)

    const submitBtn = wrapper.find('[data-testid="expense-form-submit"]')
    expect((submitBtn.element as HTMLButtonElement).disabled).toBe(true)

    expect(wrapper.text()).toContain('Faltan ₡50.00 por asignar')
  })
})

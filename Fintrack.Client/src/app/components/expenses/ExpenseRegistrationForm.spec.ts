import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi } from 'vitest'
import ExpenseRegistrationForm from './ExpenseRegistrationForm.vue'
import api from '@/services/api'

// Mock the API to prevent actual network calls during the UI unit test
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
    
    // Fill out Merchant but NOT Amount
    await wrapper.find('input[placeholder="Ej: AutoMercado"]').setValue('Test Merchant')
    await wrapper.find('input[type="date"]').setValue('2025-01-01')
    await wrapper.find('select').setValue(1)
    
    const submitBtn = wrapper.find('button[type="submit"]')
    expect((submitBtn.element as HTMLButtonElement).disabled).toBe(true)
  })

  it('Should emit submit event when valid simple form is submitted', async () => {
    vi.mocked(api.get).mockResolvedValue({ data: mockCategories })
    const wrapper = mount(ExpenseRegistrationForm)
    await flushPromises()
    
    await wrapper.find('input[placeholder="Ej: AutoMercado"]').setValue('Test Merchant')
    await wrapper.find('input[type="date"]').setValue('2025-01-01')
    await wrapper.find('input[type="number"]').setValue(100)
    await wrapper.find('select').setValue(1)
    
    await wrapper.find('form').trigger('submit')
    
    const emitted = wrapper.emitted('submit')
    expect(emitted).toBeTruthy()
    expect(emitted?.[0]?.[0]).toMatchObject({
      merchant: 'Test Merchant',
      totalAmount: 100,
      items: [
        { categoryId: 1, itemAmount: 100 }
      ]
    })
  })

  it('Should_DisableSubmitButton_When_RunningSum_DoesNotMatch_TotalAmount in Itemized Mode', async () => {
    const wrapper = mount(ExpenseRegistrationForm)
    
    // Switch to Split / Itemized Mode
    // The button has a call_split icon text, let's find it.
    const splitBtn = wrapper.findAll('button').find((b: any) => b.text().includes('Desglosar en múltiples categorías'))
    await splitBtn!.trigger('click')

    // Find the header total amount input (the class contains text-xl font-bold per design)
    // Actually the design does not put an input for the total amount inside the split mode, it just displays it.
    // Wait, in my vue implementation, the total amount is only input in the simple mode wrapper. If it is switched, it uses the provided total wrapper.
    // So let's provide the total amount first.
    
    // Turn off split mode to provide data
    await wrapper.findAll('button').find((b: any) => b.text().includes('Volver a modo simple'))!.trigger('click')

    await wrapper.find('input[placeholder="Ej: AutoMercado"]').setValue('Test Merchant')
    await wrapper.find('input[type="date"]').setValue('2025-01-01')
    await wrapper.find('input[type="number"]').setValue(100)

    // Re-enter Split / Itemized mode
    await wrapper.findAll('button').find((b: any) => b.text().includes('Desglosar en múltiples categorías'))!.trigger('click')

    // Find the first row's amount input. It has type="number" and step="0.01"
    const itemAmountInput = wrapper.findAll('input[type="number"][step="0.01"]')[0]
    await itemAmountInput!.setValue(50.00) // Sum is 50, but total is 100

    const submitBtn = wrapper.find('button[type="submit"]')
    expect((submitBtn.element as HTMLButtonElement).disabled).toBe(true)
    
    // Check blackbox rendering text
    expect(wrapper.text()).toContain('Faltan ₡50.00 por asignar')
  })

})

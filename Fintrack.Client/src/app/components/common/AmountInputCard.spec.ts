import { mount } from '@vue/test-utils'
import { describe, it, expect } from 'vitest'
import AmountInputCard from './AmountInputCard.vue'

describe('AmountInputCard.vue', () => {
  it('renders currency symbol and a number input inside SurfaceCard', () => {
    const wrapper = mount(AmountInputCard, {
      props: { modelValue: null },
    })
    expect(wrapper.text()).toContain('₡')
    expect(wrapper.find('input[type="number"]').exists()).toBe(true)
  })
})

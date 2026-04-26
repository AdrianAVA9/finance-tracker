import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { createRouter, createMemoryHistory } from 'vue-router'
import CategoryManagementView from './CategoryManagementView.vue'
import api from '@/services/api'

vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn()
  }
}))

async function mountWithRouteName(name: 'SettingsIncomeCategories' | 'SettingsExpenseCategories') {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      {
        path: '/',
        name,
        component: CategoryManagementView,
        meta: { subtitle: 'Subtítulo de prueba' }
      }
    ]
  })
  await router.push('/')
  await router.isReady()
  const wrapper = mount(CategoryManagementView, {
    global: { plugins: [router] }
  })
  return { wrapper, router }
}

describe('CategoryManagementView.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders user-owned income categories when API returns data', async () => {
    vi.mocked(api.get).mockResolvedValue({
      data: [
        {
          id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
          name: 'Ingreso personalizado',
          icon: 'savings',
          color: '#00FF99',
          isEditable: true
        }
      ]
    })

    const { wrapper } = await mountWithRouteName('SettingsIncomeCategories')
    await flushPromises()

    expect(vi.mocked(api.get)).toHaveBeenCalledWith('/api/v1/incomes/categories/owned')
    expect(wrapper.text()).toContain('Ingreso personalizado')
  })

  it('renders user-owned expense categories grouped by group name', async () => {
    vi.mocked(api.get).mockResolvedValue({
      data: [
        {
          id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
          name: 'Categoría A',
          icon: 'shopping_bag',
          group: { name: 'Hogar' }
        }
      ]
    })

    const { wrapper } = await mountWithRouteName('SettingsExpenseCategories')
    await flushPromises()

    expect(vi.mocked(api.get)).toHaveBeenCalledWith('/api/v1/expensecategories/owned')
    expect(wrapper.text()).toContain('Hogar')
    expect(wrapper.text()).toContain('Categoría A')
  })

  it('filters income categories by search', async () => {
    vi.mocked(api.get).mockResolvedValue({
      data: [
        { id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', name: 'Uno', isEditable: true },
        { id: 'cccccccc-cccc-cccc-cccc-cccccccccccc', name: 'Dos', isEditable: true }
      ]
    })

    const { wrapper } = await mountWithRouteName('SettingsIncomeCategories')
    await flushPromises()

    const searchInput = wrapper.get('input[type="search"]')
    await searchInput.setValue('Dos')

    expect(wrapper.text()).toContain('Dos')
    expect(wrapper.text()).not.toContain('Uno')
  })
})

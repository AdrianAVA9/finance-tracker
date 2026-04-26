import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { createRouter, createMemoryHistory, type RouteRecordRaw } from 'vue-router'
import CategoryFormView from './CategoryFormView.vue'
import api from '@/services/api'

vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn()
  }
}))

const categoryFormRoutes: RouteRecordRaw[] = [
  { path: '/settings/categories/income/new', name: 'CategoryIncomeNew', component: CategoryFormView },
  { path: '/settings/categories/income/:categoryId/edit', name: 'CategoryIncomeEdit', component: CategoryFormView },
  { path: '/settings/categories/expense/new', name: 'CategoryExpenseNew', component: CategoryFormView },
  {
    path: '/settings/categories/expense/:categoryId/edit',
    name: 'CategoryExpenseEdit',
    component: CategoryFormView
  }
]

async function mountOnRoute(
  to: { name: 'CategoryIncomeNew' } | { name: 'CategoryExpenseNew' } | { name: 'CategoryIncomeEdit'; params: { categoryId: string } } | { name: 'CategoryExpenseEdit'; params: { categoryId: string } }
) {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: categoryFormRoutes
  })
  await router.push(to)
  await router.isReady()
  const wrapper = mount(CategoryFormView, { global: { plugins: [router] } })
  await flushPromises()
  return { wrapper, router }
}

describe('CategoryFormView.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('shows Tipo de rubro Ingreso for new income and does not request expense groups', async () => {
    const { wrapper } = await mountOnRoute({ name: 'CategoryIncomeNew' })

    expect(wrapper.text()).toContain('Tipo de rubro')
    expect(wrapper.text()).toContain('Ingreso')
    expect(wrapper.text()).not.toContain('Descripción')
    expect(vi.mocked(api.get)).not.toHaveBeenCalled()
  })

  it('submits new income with POST /api/v1/incomes/categories and navigates to income list', async () => {
    vi.mocked(api.post).mockResolvedValue({ data: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa' })
    const { wrapper, router } = await mountOnRoute({ name: 'CategoryIncomeNew' })
    const pushSpy = vi.spyOn(router, 'push')

    await wrapper.get('#cat-name').setValue('Mi ingreso')
    await wrapper.get('form').trigger('submit.prevent')
    await flushPromises()

    expect(vi.mocked(api.post)).toHaveBeenCalledWith('/api/v1/incomes/categories', {
      name: 'Mi ingreso',
      icon: 'restaurant',
      color: '#05E699'
    })
    expect(pushSpy).toHaveBeenCalledWith({ name: 'SettingsIncomeCategories' })
  })

  it('loads groups for new expense, shows Descripción, and posts with groupId', async () => {
    vi.mocked(api.get).mockResolvedValue({
      data: [{ id: 'f81c32ed-4235-47e5-be74-e1ebfc7b5519', name: 'Comunicaciones' }]
    })
    vi.mocked(api.post).mockResolvedValue({ data: 'd6f752c4-442e-4d06-af6d-4bc9c1cd5b66' })

    const { wrapper, router } = await mountOnRoute({ name: 'CategoryExpenseNew' })
    const pushSpy = vi.spyOn(router, 'push')

    expect(vi.mocked(api.get)).toHaveBeenCalledWith('/api/v1/expensecategorygroups')
    expect(wrapper.text()).toContain('Gasto')
    expect(wrapper.text()).toContain('Descripción')
    expect(wrapper.text()).toContain('Asignar a grupo')

    await wrapper.get('#cat-name').setValue('Recargas')
    await wrapper.get('#cat-desc').setValue('Texto de prueba')
    await wrapper.get('form').trigger('submit.prevent')
    await flushPromises()

    expect(vi.mocked(api.post)).toHaveBeenCalledWith('/api/v1/expensecategories', {
      name: 'Recargas',
      description: 'Texto de prueba',
      icon: 'restaurant',
      color: '#05E699',
      groupId: 'f81c32ed-4235-47e5-be74-e1ebfc7b5519'
    })
    expect(pushSpy).toHaveBeenCalledWith({ name: 'SettingsExpenseCategories' })
  })

  it('loads existing expense category and PUT on save', async () => {
    const id = 'd6f752c4-442e-4d06-af6d-4bc9c1cd5b66'
    vi.mocked(api.get)
      .mockResolvedValueOnce({
        data: [{ id: 'f81c32ed-4235-47e5-be74-e1ebfc7b5519', name: 'Comunicaciones' }]
      })
      .mockResolvedValueOnce({
        data: {
          id,
          name: 'Recargas Prepago',
          description: 'Notas',
          icon: 'phone',
          color: '#05E699',
          groupId: 'f81c32ed-4235-47e5-be74-e1ebfc7b5519',
          isEditable: true
        }
      })
    vi.mocked(api.put).mockResolvedValue({ data: null })

    const { wrapper, router } = await mountOnRoute({ name: 'CategoryExpenseEdit', params: { categoryId: id } })
    const pushSpy = vi.spyOn(router, 'push')
    await flushPromises()

    expect(vi.mocked(api.get)).toHaveBeenCalledWith(`/api/v1/expensecategories/${id}`)

    const nameField = wrapper.get('#cat-name')
    expect((nameField.element as HTMLInputElement).value).toBe('Recargas Prepago')
    await nameField.setValue('Recargas Prepago 2')
    await wrapper.get('form').trigger('submit.prevent')
    await flushPromises()

    expect(vi.mocked(api.put)).toHaveBeenCalledWith(`/api/v1/expensecategories/${id}`, {
      name: 'Recargas Prepago 2',
      description: 'Notas',
      icon: 'phone',
      color: '#05E699',
      groupId: 'f81c32ed-4235-47e5-be74-e1ebfc7b5519'
    })
    expect(pushSpy).toHaveBeenCalledWith({ name: 'SettingsExpenseCategories' })
  })

  it('shows validation error when name is empty on submit', async () => {
    const { wrapper } = await mountOnRoute({ name: 'CategoryIncomeNew' })

    await wrapper.get('form').trigger('submit.prevent')
    await flushPromises()

    expect(wrapper.text()).toContain('El nombre de la categoría es obligatorio.')
    expect(vi.mocked(api.post)).not.toHaveBeenCalled()
  })
})

import { defineComponent, ref, nextTick } from 'vue'
import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useAppOfflineFullPage } from '@/composables/useAppOfflineFullPage'

const mockPath = ref('/app/dashboard')
const mockIsOnline = ref(true)

vi.mock('vue-router', () => ({
  useRoute: () =>
    ({
      get path() {
        return mockPath.value
      },
    }) as { path: string },
}))

vi.mock('@/shared/composables/useNetworkStatus', () => ({
  useNetworkStatus: () => ({
    isOnline: mockIsOnline,
  }),
}))

const TestHost = defineComponent({
  setup() {
    return useAppOfflineFullPage()
  },
  template: '<div />',
})

describe('useAppOfflineFullPage', () => {
  beforeEach(() => {
    mockPath.value = '/app/dashboard'
    mockIsOnline.value = true
  })

  it('starts with full-page offline hidden when online', async () => {
    const wrapper = mount(TestHost)
    await flushPromises()
    expect(wrapper.vm.showOfflinePage).toBe(false)
  })

  it('shows full-page offline on first paint when already offline', async () => {
    mockIsOnline.value = false
    const wrapper = mount(TestHost)
    await flushPromises()
    expect(wrapper.vm.showOfflinePage).toBe(true)
  })

  it('shows full-page offline after client-side navigation while offline', async () => {
    mockIsOnline.value = true
    const wrapper = mount(TestHost)
    await flushPromises()
    expect(wrapper.vm.showOfflinePage).toBe(false)

    mockIsOnline.value = false
    mockPath.value = '/app/activity'
    await nextTick()
    await flushPromises()
    expect(wrapper.vm.showOfflinePage).toBe(true)
  })

  it('hides full-page offline when connection returns', async () => {
    mockIsOnline.value = false
    const wrapper = mount(TestHost)
    await flushPromises()
    expect(wrapper.vm.showOfflinePage).toBe(true)

    mockIsOnline.value = true
    await nextTick()
    await flushPromises()
    expect(wrapper.vm.showOfflinePage).toBe(false)
  })
})

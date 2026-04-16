import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useNetworkStatus } from '@/shared/composables/useNetworkStatus'

/**
 * Full-page offline overlay for the /app domain: shows when the user navigates
 * while offline (and clears when the connection returns).
 */
export function useAppOfflineFullPage() {
  const route = useRoute()
  const { isOnline } = useNetworkStatus()
  const showOfflinePage = ref(false)

  watch(
    () => route.path,
    () => {
      if (!isOnline.value) {
        showOfflinePage.value = true
      }
    },
    { immediate: true },
  )

  watch(isOnline, (online) => {
    if (online) {
      showOfflinePage.value = false
    }
  })

  return { showOfflinePage }
}

import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useNetworkStatus } from '@/shared/composables/useNetworkStatus'

/**
 * Root shell: session bootstrap, offline overlay toggle, post-init auth redirects.
 */
export function useAppShell() {
  const router = useRouter()
  const { isInitialized, isAuthenticated, checkSession } = useAuth()
  const { isOnline } = useNetworkStatus()
  const showOfflinePage = ref(false)

  onMounted(async () => {
    if (!isInitialized.value) {
      await checkSession()
    }
  })

  watch(
    () => router.currentRoute.value.path,
    () => {
      if (!isOnline.value) {
        showOfflinePage.value = true
      }
    },
  )

  watch(isOnline, (online) => {
    if (online) {
      showOfflinePage.value = false
    }
  })

  watch(isInitialized, (initialized) => {
    if (!initialized) return

    const currentPath = router.currentRoute.value.path
    const isAuthDomain = currentPath.startsWith('/auth')
    const isAppDomain = currentPath.startsWith('/app')
    const isPublicDomain = !isAuthDomain && !isAppDomain && !currentPath.startsWith('/admin')

    if ((isPublicDomain || isAuthDomain) && isAuthenticated.value) {
      void router.replace('/app')
    } else if (isAppDomain && !isAuthenticated.value) {
      void router.replace('/auth/login')
    }
  })

  return {
    isInitialized,
    showOfflinePage,
  }
}

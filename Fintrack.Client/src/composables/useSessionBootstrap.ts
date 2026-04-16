import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { getAuthRedirectForPath } from '@/router/authRedirect'

/**
 * Global session bootstrap: ensures checkSession runs once, then aligns the current
 * URL with auth state using the same rules as router beforeEach.
 */
export function useSessionBootstrap() {
  const router = useRouter()
  const { isInitialized, isAuthenticated, checkSession } = useAuth()

  onMounted(async () => {
    if (!isInitialized.value) {
      await checkSession()
    }
    const path = router.currentRoute.value.path
    const redirect = getAuthRedirectForPath(path, isAuthenticated.value)
    if (redirect) {
      void router.replace(redirect)
    }
  })

  return { isInitialized }
}

/**
 * True when the app runs as an installed PWA (home screen / "Add to Home Screen").
 * - `(display-mode: standalone|fullscreen)`: standard signal (Android PWA, modern browsers).
 * - `navigator.standalone` on iOS Safari when opened from the home screen icon.
 */
export function isInstalledPwaDisplayMode(): boolean {
  if (typeof window === 'undefined') return false
  if (window.matchMedia('(display-mode: standalone)').matches) return true
  if (window.matchMedia('(display-mode: fullscreen)').matches) return true
  const { standalone } = window.navigator as Navigator & { standalone?: boolean }
  return standalone === true
}

/**
 * When the installed app opens, avoid showing marketing/public routes; send users to
 * the app entry so auth rules can send them to login or the dashboard.
 */
export function getInstalledPwaRedirectForPath(path: string): string | null {
  if (!isInstalledPwaDisplayMode()) return null
  const isAuthDomain = path.startsWith('/auth')
  const isAppDomain = path.startsWith('/app')
  const isAdminDomain = path.startsWith('/admin')
  const isPublicDomain = !isAuthDomain && !isAppDomain && !isAdminDomain
  if (isPublicDomain) return '/app'
  return null
}

/**
 * Canonical auth-vs-route rules shared by the router guard and post-session bootstrap.
 * Keep in sync with router beforeEach in router/index.ts.
 */
export function getAuthRedirectForPath(
  path: string,
  isAuthenticated: boolean,
): string | null {
  const isAuthDomain = path.startsWith('/auth')
  const isAppDomain = path.startsWith('/app')
  const isAdminDomain = path.startsWith('/admin')
  const isPublicDomain = !isAuthDomain && !isAppDomain && !isAdminDomain

  if ((isPublicDomain || isAuthDomain) && isAuthenticated) {
    return '/app'
  }

  if ((isAppDomain || isAdminDomain) && !isAuthenticated) {
    return '/auth/login'
  }

  return null
}

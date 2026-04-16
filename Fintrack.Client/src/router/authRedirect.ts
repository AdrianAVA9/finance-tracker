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

import { describe, it, expect, vi, afterEach } from 'vitest'
import {
  getAuthRedirectForPath,
  getInstalledPwaRedirectForPath,
  isInstalledPwaDisplayMode,
} from '@/router/authRedirect'

describe('getAuthRedirectForPath', () => {
  it('sends authenticated users away from public paths to /app', () => {
    expect(getAuthRedirectForPath('/', true)).toBe('/app')
    expect(getAuthRedirectForPath('/pricing', true)).toBe('/app')
  })

  it('sends authenticated users away from auth paths to /app', () => {
    expect(getAuthRedirectForPath('/auth/login', true)).toBe('/app')
  })

  it('does not redirect authenticated users already in /app', () => {
    expect(getAuthRedirectForPath('/app/dashboard', true)).toBeNull()
  })

  it('sends unauthenticated users from /app to login', () => {
    expect(getAuthRedirectForPath('/app/dashboard', false)).toBe('/auth/login')
  })

  it('sends unauthenticated users from /admin to login', () => {
    expect(getAuthRedirectForPath('/admin/users', false)).toBe('/auth/login')
  })

  it('allows unauthenticated users on public and auth routes', () => {
    expect(getAuthRedirectForPath('/', false)).toBeNull()
    expect(getAuthRedirectForPath('/auth/login', false)).toBeNull()
  })
})

describe('installed PWA routing', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  function stubStandaloneDisplay() {
    vi.stubGlobal(
      'matchMedia',
      vi.fn().mockImplementation((query: string) => ({
        matches: query.includes('standalone') || query.includes('fullscreen'),
        media: query,
        addEventListener: vi.fn(),
        removeEventListener: vi.fn(),
        addListener: vi.fn(),
        removeListener: vi.fn(),
        dispatchEvent: vi.fn(),
      })),
    )
  }

  it('detects installed PWA via display-mode standalone', () => {
    stubStandaloneDisplay()
    expect(isInstalledPwaDisplayMode()).toBe(true)
  })

  it('redirects public paths to /app when in installed display mode', () => {
    stubStandaloneDisplay()
    expect(getInstalledPwaRedirectForPath('/')).toBe('/app')
  })

  it('does not redirect when not in installed display mode', () => {
    vi.stubGlobal('matchMedia', () => ({ matches: false }) as MediaQueryList)
    expect(getInstalledPwaRedirectForPath('/')).toBeNull()
  })

  it('does not redirect app or auth paths in installed mode', () => {
    stubStandaloneDisplay()
    expect(getInstalledPwaRedirectForPath('/app')).toBeNull()
    expect(getInstalledPwaRedirectForPath('/auth/login')).toBeNull()
  })
})

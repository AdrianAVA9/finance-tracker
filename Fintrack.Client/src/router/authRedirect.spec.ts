import { describe, it, expect } from 'vitest'
import { getAuthRedirectForPath } from '@/router/authRedirect'

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

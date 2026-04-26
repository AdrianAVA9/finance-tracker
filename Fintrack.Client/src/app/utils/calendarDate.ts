/**
 * Parses API ISO timestamps as a **calendar** date (purchase / transaction day).
 * Values like `2026-04-03T00:00:00Z` must not shift to the previous local day when formatting.
 */
export function parseCalendarDateFromApi(isoDate: string): Date {
  const trimmed = isoDate.trim()
  const dayPart = trimmed.slice(0, 10)
  if (/^\d{4}-\d{2}-\d{2}$/.test(dayPart)) {
    const parts = dayPart.split('-').map(Number)
    const y = parts[0]!
    const m = parts[1]!
    const d = parts[2]!
    return new Date(y, m - 1, d)
  }
  const instant = new Date(trimmed)
  if (Number.isFinite(instant.getTime())) {
    return new Date(instant.getUTCFullYear(), instant.getUTCMonth(), instant.getUTCDate())
  }
  const [y, m, d] = (trimmed.split('T')[0] || '').split('-').map(Number)
  return new Date(y || 0, (m || 1) - 1, d || 1)
}

/** True when the instant is midnight UTC (typical date-only field from the API). */
export function isUtcMidnightInstant(isoDate: string): boolean {
  const instant = new Date(isoDate.trim())
  if (!Number.isFinite(instant.getTime())) return false
  return (
    instant.getUTCHours() === 0 &&
    instant.getUTCMinutes() === 0 &&
    instant.getUTCSeconds() === 0 &&
    instant.getUTCMilliseconds() === 0
  )
}

import { isUtcMidnightInstant, parseCalendarDateFromApi } from '@/app/utils/calendarDate'

export const formatDashboardCurrency = (value: number): string =>
  new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(value)

export const formatDashboardDate = (dateString: string): string => {
  const cal = parseCalendarDateFromApi(dateString)
  const instant = new Date(dateString.trim())
  const now = new Date()
  const isToday = cal.toDateString() === now.toDateString()
  const dateOnly = isUtcMidnightInstant(dateString)

  if (isToday) {
    if (dateOnly || !Number.isFinite(instant.getTime())) {
      return 'Hoy'
    }
    return `Hoy, ${instant.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`
  }

  const datePart = cal.toLocaleDateString('es-ES', {
    month: 'short',
    day: '2-digit',
    year: cal.getFullYear() !== now.getFullYear() ? 'numeric' : undefined,
  })

  if (dateOnly || !Number.isFinite(instant.getTime())) {
    return datePart
  }

  return `${datePart}, ${instant.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`
}

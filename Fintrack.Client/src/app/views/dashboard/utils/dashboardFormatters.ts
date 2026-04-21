export const formatDashboardCurrency = (value: number): string =>
  new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(value)

export const formatDashboardDate = (dateString: string): string => {
  const date = new Date(dateString)
  const now = new Date()
  const isToday = date.toDateString() === now.toDateString()

  if (isToday) {
    return `Hoy, ${date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`
  }

  return (
    date.toLocaleDateString('es-ES', {
      month: 'short',
      day: '2-digit',
      year: date.getFullYear() !== now.getFullYear() ? 'numeric' : undefined,
    }) + `, ${date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`
  )
}

export const money = (n) =>
  new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB', maximumFractionDigits: 0 }).format(n)

// Сервер отдаёт время в UTC без суффикса Z
export function dateTime(value) {
  if (!value) return ''
  const iso = /[zZ]|[+-]\d\d:?\d\d$/.test(value) ? value : `${value}Z`
  return new Date(iso).toLocaleString('ru-RU', { dateStyle: 'medium', timeStyle: 'short' })
}

export const orderTotal = (order) =>
  (order.items ?? []).reduce((sum, i) => sum + i.price * i.quantity, 0)

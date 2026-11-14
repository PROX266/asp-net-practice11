export const API_URL = import.meta.env.VITE_API_URL ?? 'https://localhost:7276'

const TOKEN_KEY = 'webstore_access_token'

export const getToken = () => localStorage.getItem(TOKEN_KEY)
export const setToken = (t) => localStorage.setItem(TOKEN_KEY, t)
export const clearToken = () => localStorage.removeItem(TOKEN_KEY)

export function decodeToken(token) {
  try {
    const part = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const json = new TextDecoder().decode(
      Uint8Array.from(atob(part), (c) => c.charCodeAt(0))
    )
    const p = JSON.parse(json)
    return {
      id: Number(p.nameid ?? p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']),
      login: p.unique_name ?? p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      role: p.role ?? p['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
      exp: p.exp ? new Date(p.exp * 1000) : null,
    }
  } catch {
    return null
  }
}

async function diagnoseNetworkError() {
  try {
    await fetch(`${API_URL}/api/products?pageSize=1`, { mode: 'no-cors' })
    return 'cors'
  } catch {
    return 'unreachable'
  }
}

function classify(status) {
  if (status >= 200 && status < 300) return 'ok'
  if (status === 400) return 'badRequest'
  if (status === 401) return 'unauthorized'
  if (status === 403) return 'forbidden'
  if (status === 404) return 'notFound'
  if (status === 429) return 'rateLimited'
  return 'server'
}

export async function apiRequest(path, { method = 'GET', body, auth = true } = {}) {
  const headers = {}
  if (body !== undefined) headers['Content-Type'] = 'application/json'
  const token = getToken()
  if (auth && token) headers['Authorization'] = `Bearer ${token}`
  const sentToken = Boolean(auth && token)

  let response
  try {
    response = await fetch(`${API_URL}${path}`, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    })
  } catch {
    const kind = await diagnoseNetworkError()
    return { ok: false, status: 0, kind, data: null, sentToken }
  }

  const text = await response.text()
  let data = null
  if (text) {
    try {
      data = JSON.parse(text)
    } catch {
      data = text
    }
  }

  return {
    ok: response.ok,
    status: response.status,
    kind: classify(response.status),
    data,
    sentToken,
    total: Number(response.headers.get('X-Total-Count') ?? 0),
    retryAfter: Number(response.headers.get('Retry-After') ?? 0),
  }
}

export function failureText(res, overrides = {}) {
  if (overrides[res.kind]) return overrides[res.kind]
  switch (res.kind) {
    case 'badRequest':
      return (res.data && res.data.error) || 'Проверьте правильность введённых данных.'
    case 'unauthorized':
      return res.sentToken
        ? 'Сессия истекла или недействительна. Войдите снова.'
        : 'Нужно войти в аккаунт.'
    case 'forbidden':
      return 'Недостаточно прав для этого действия.'
    case 'notFound':
      return 'Ничего не найдено.'
    case 'rateLimited':
      return res.retryAfter
        ? `Слишком много попыток. Повторите через ${res.retryAfter} с.`
        : 'Слишком много попыток. Повторите позже.'
    case 'cors':
      return `Браузер заблокировал запрос: сервер не разрешает обращения с адреса ${window.location.origin} (политика CORS).`
    case 'unreachable':
      return 'Не удаётся связаться с сервером. Проверьте, что API запущен и сертификат https доверенный.'
    default:
      return 'Ошибка на сервере. Попробуйте позже.'
  }
}

export function codeLabel(res) {
  if (res.kind === 'cors') return 'Ошибка CORS'
  if (res.kind === 'unreachable') return 'Нет соединения'
  return `Код ответа ${res.status}`
}

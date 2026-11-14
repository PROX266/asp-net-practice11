import { useCallback, useMemo, useState } from 'react'
import { apiRequest, clearToken, codeLabel, decodeToken, failureText, getToken, setToken } from './api.js'
import AdminOrders from './AdminOrders.jsx'
import Catalog from './Catalog.jsx'
import LoginScreen from './LoginScreen.jsx'
import Orders from './Orders.jsx'
import Toasts from './Toasts.jsx'

function initialToken() {
  const t = getToken()
  if (!t) return null
  const info = decodeToken(t)
  if (!info || (info.exp && info.exp < new Date())) {
    clearToken()
    return null
  }
  return t
}

const ROLE_TITLES = { Admin: 'Администратор', Manager: 'Менеджер', User: 'Покупатель' }

export default function App() {
  const [token, setTokenState] = useState(initialToken)
  const [view, setView] = useState('catalog')
  const [toasts, setToasts] = useState([])

  const user = useMemo(() => (token ? decodeToken(token) : null), [token])

  const closeToast = useCallback((id) => setToasts((p) => p.filter((t) => t.id !== id)), [])

  const notify = useCallback((type, text, code) => {
    const id = Date.now() + Math.random()
    setToasts((p) => [...p, { id, type, text, code }])
    setTimeout(() => setToasts((p) => p.filter((t) => t.id !== id)), 7000)
  }, [])

  const logout = useCallback(() => {
    clearToken()
    setTokenState(null)
    setView('catalog')
  }, [])

  function onAuthenticated(accessToken) {
    setToken(accessToken)
    setTokenState(accessToken)
    setView('catalog')
  }

  const call = useCallback(
    async (path, options = {}) => {
      const { messages, silent, ...rest } = options
      const res = await apiRequest(path, rest)
      if (!res.ok && !silent) {
        notify('error', failureText(res, messages), codeLabel(res))
        if (res.kind === 'unauthorized' && res.sentToken) logout()
      }
      return res
    },
    [notify, logout]
  )

  if (!user) {
    return (
      <>
        <LoginScreen onAuthenticated={onAuthenticated} />
        <Toasts items={toasts} onClose={closeToast} />
      </>
    )
  }

  const nav = [
    { id: 'catalog', title: 'Каталог' },
    { id: 'orders', title: 'Мои заказы' },
    { id: 'admin', title: 'Администрирование' },
  ]

  return (
    <div className="shell">
      <header className="topbar">
        <div className="brand">Webstore</div>
        <nav>
          {nav.map((n) => (
            <button
              key={n.id}
              className={view === n.id ? 'nav-item active' : 'nav-item'}
              onClick={() => setView(n.id)}
            >
              {n.title}
            </button>
          ))}
        </nav>
        <div className="user">
          <div className="user-info">
            <div className="user-name">{user.login}</div>
            <div className="muted small">{ROLE_TITLES[user.role] ?? user.role}</div>
          </div>
          <button className="ghost" onClick={logout}>Выйти</button>
        </div>
      </header>

      <main className="content">
        {view === 'catalog' && <Catalog call={call} />}
        {view === 'orders' && <Orders call={call} user={user} notify={notify} />}
        {view === 'admin' && <AdminOrders call={call} />}
      </main>

      <footer className="footer">&copy; {new Date().getFullYear()} Webstore</footer>
      <Toasts items={toasts} onClose={closeToast} />
    </div>
  )
}

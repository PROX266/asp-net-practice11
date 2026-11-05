import { useEffect, useState } from 'react'
import { apiRequest, failureText } from './api.js'

export default function LoginScreen({ onAuthenticated }) {
  const [mode, setMode] = useState('login')
  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const [lockSeconds, setLockSeconds] = useState(0)

  // Пока действует блокировка rate limiting, кнопка неактивна и показывает обратный отсчёт
  useEffect(() => {
    if (lockSeconds <= 0) return
    const id = setTimeout(() => setLockSeconds((s) => s - 1), 1000)
    return () => clearTimeout(id)
  }, [lockSeconds])

  async function submit(e) {
    e.preventDefault()
    setError('')
    setBusy(true)
    const res = await apiRequest(`/api/auth/${mode}`, {
      method: 'POST',
      body: { login: login.trim(), password },
      auth: false,
    })
    setBusy(false)

    if (res.ok && res.data?.accessToken) {
      onAuthenticated(res.data.accessToken)
      return
    }

    if (res.kind === 'rateLimited') setLockSeconds(res.retryAfter || 60)

    setError(
      failureText(res, {
        unauthorized: 'Неверный логин или пароль.',
        badRequest:
          res.data?.error ||
          (res.data?.errors && Object.values(res.data.errors).flat()[0]) ||
          'Проверьте правильность введённых данных.',
      })
    )
  }

  const locked = lockSeconds > 0

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="brand brand-lg">Webstore</div>
        <p className="muted">Интернет-магазин электроники</p>

        <div className="tabs">
          <button
            type="button"
            className={mode === 'login' ? 'tab active' : 'tab'}
            onClick={() => { setMode('login'); setError('') }}
          >
            Вход
          </button>
          <button
            type="button"
            className={mode === 'register' ? 'tab active' : 'tab'}
            onClick={() => { setMode('register'); setError('') }}
          >
            Регистрация
          </button>
        </div>

        <form onSubmit={submit}>
          <label>
            Логин
            <input
              value={login}
              onChange={(e) => setLogin(e.target.value)}
              autoComplete="username"
              required
              minLength={mode === 'register' ? 3 : undefined}
              maxLength={50}
            />
          </label>
          <label>
            Пароль
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
              required
              minLength={mode === 'register' ? 6 : undefined}
            />
          </label>

          {error && <div className="form-error">{error}</div>}

          <button className="primary wide" disabled={busy || locked}>
            {locked
              ? `Повторите через ${lockSeconds} с`
              : busy
                ? 'Подождите...'
                : mode === 'login'
                  ? 'Войти'
                  : 'Создать аккаунт'}
          </button>
        </form>
      </div>
    </div>
  )
}

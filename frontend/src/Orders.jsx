import { useCallback, useEffect, useState } from 'react'
import OrderCard from './OrderCard.jsx'

export default function Orders({ call, user, notify }) {
  const [orders, setOrders] = useState([])
  const [loading, setLoading] = useState(true)
  const [number, setNumber] = useState('')
  const [found, setFound] = useState(null)

  const load = useCallback(async () => {
    const res = await call('/api/orders/my')
    if (res.ok) setOrders(res.data)
    setLoading(false)
  }, [call])

  useEffect(() => { load() }, [load])

  async function create() {
    const res = await call('/api/orders', { method: 'POST', body: { userId: user.id } })
    if (res.ok) {
      notify('success', `Заказ № ${res.data.id} оформлен.`)
      load()
    }
  }

  async function cancel(order) {
    if (!window.confirm(`Отменить заказ № ${order.id}?`)) return
    const res = await call(`/api/orders/${order.id}`, {
      method: 'DELETE',
      messages: { forbidden: 'Этот заказ принадлежит другому пользователю.' },
    })
    if (res.ok) {
      notify('success', `Заказ № ${order.id} отменён.`)
      if (found?.id === order.id) setFound(null)
      load()
    }
  }

  async function search(e) {
    e.preventDefault()
    setFound(null)
    const res = await call(`/api/orders/${number.trim()}`, {
      messages: {
        forbidden: 'Этот заказ принадлежит другому пользователю.',
        notFound: 'Заказ с таким номером не найден.',
      },
    })
    if (res.ok) setFound(res.data)
  }

  return (
    <section>
      <div className="section-head">
        <h2>Мои заказы</h2>
        <button className="primary" onClick={create}>Оформить заказ</button>
      </div>

      <form className="search" onSubmit={search}>
        <input
          type="number"
          min="1"
          placeholder="Найти заказ по номеру"
          value={number}
          onChange={(e) => setNumber(e.target.value)}
          required
        />
        <button>Найти</button>
      </form>

      {found && (
        <div className="found">
          <div className="muted small">Результат поиска</div>
          <OrderCard order={found} showOwner />
        </div>
      )}

      {loading && <p className="muted">Загрузка...</p>}
      {!loading && orders.length === 0 && (
        <p className="muted">У вас пока нет заказов.</p>
      )}

      <div className="list">
        {orders.map((o) => (
          <OrderCard key={o.id} order={o} onDelete={cancel} />
        ))}
      </div>
    </section>
  )
}

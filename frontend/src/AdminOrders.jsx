import { useEffect, useState } from 'react'
import OrderCard from './OrderCard.jsx'

const PAGE_SIZE = 6

export default function AdminOrders({ call }) {
  const [page, setPage] = useState(1)
  const [orders, setOrders] = useState([])
  const [total, setTotal] = useState(0)
  const [loading, setLoading] = useState(true)
  const [denied, setDenied] = useState(false)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    call(`/api/orders?page=${page}&pageSize=${PAGE_SIZE}`, {
      messages: { forbidden: 'Раздел доступен только администраторам.' },
    }).then((res) => {
      if (cancelled) return
      if (res.ok) {
        setOrders(res.data)
        setTotal(res.total)
        setDenied(false)
      } else if (res.status === 403) {
        setDenied(true)
      }
      setLoading(false)
    })
    return () => { cancelled = true }
  }, [page, call])

  const pages = Math.max(1, Math.ceil(total / PAGE_SIZE))

  if (denied) {
    return (
      <section>
        <div className="section-head"><h2>Администрирование</h2></div>
        <div className="empty-state">
          <div className="empty-title">Доступ ограничен</div>
          <p className="muted">Просмотр заказов всех пользователей доступен только администраторам.</p>
        </div>
      </section>
    )
  }

  return (
    <section>
      <div className="section-head">
        <h2>Все заказы</h2>
        {total > 0 && <span className="muted">Всего: {total}</span>}
      </div>

      {loading && <p className="muted">Загрузка...</p>}
      {!loading && orders.length === 0 && <p className="muted">Заказов пока нет.</p>}

      <div className="list">
        {orders.map((o) => (
          <OrderCard key={o.id} order={o} showOwner />
        ))}
      </div>

      {pages > 1 && (
        <div className="pager">
          <button disabled={page <= 1} onClick={() => setPage(page - 1)}>Назад</button>
          <span className="muted">Страница {page} из {pages}</span>
          <button disabled={page >= pages} onClick={() => setPage(page + 1)}>Далее</button>
        </div>
      )}
    </section>
  )
}

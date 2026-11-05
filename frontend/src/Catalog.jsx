import { useEffect, useState } from 'react'
import { API_URL } from './api.js'
import { money } from './format.js'

const PAGE_SIZE = 8

export default function Catalog({ call }) {
  const [page, setPage] = useState(1)
  const [items, setItems] = useState([])
  const [total, setTotal] = useState(0)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    call(`/api/products?page=${page}&pageSize=${PAGE_SIZE}`, { auth: false }).then((res) => {
      if (cancelled) return
      if (res.ok) {
        setItems(res.data)
        setTotal(res.total)
      }
      setLoading(false)
    })
    return () => { cancelled = true }
  }, [page, call])

  const pages = Math.max(1, Math.ceil(total / PAGE_SIZE))

  return (
    <section>
      <div className="section-head">
        <h2>Каталог</h2>
        {total > 0 && <span className="muted">Товаров: {total}</span>}
      </div>

      {loading && <p className="muted">Загрузка...</p>}
      {!loading && items.length === 0 && <p className="muted">Товары не найдены.</p>}

      <div className="grid">
        {items.map((p) => (
          <article key={p.id} className="product">
            <div className="product-img">
              <img
                src={`${API_URL}${p.imagePath || '/images/no-image.svg'}`}
                alt={p.productName}
                onError={(e) => { e.currentTarget.style.visibility = 'hidden' }}
              />
            </div>
            <div className="product-info">
              <div className="product-cat">{p.category?.nameCategories}</div>
              <div className="product-name">{p.productName}</div>
              <div className="product-bottom">
                <span className="price">{money(p.price)}</span>
                <span className={p.stockQuantity > 0 ? 'stock' : 'stock out'}>
                  {p.stockQuantity > 0 ? `В наличии: ${p.stockQuantity}` : 'Нет в наличии'}
                </span>
              </div>
            </div>
          </article>
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

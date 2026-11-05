import { dateTime, money, orderTotal } from './format.js'

export default function OrderCard({ order, showOwner, onDelete }) {
  const items = order.items ?? []
  return (
    <article className="order">
      <div className="order-head">
        <div>
          <div className="order-title">Заказ № {order.id}</div>
          <div className="muted small">
            {dateTime(order.createdAt)}
            {showOwner && order.user && <> · {order.user.userLogin}</>}
          </div>
        </div>
        <div className="order-sum">{money(orderTotal(order))}</div>
      </div>

      {items.length === 0 ? (
        <p className="muted small">Состав заказа пока пуст.</p>
      ) : (
        <ul className="order-items">
          {items.map((i) => (
            <li key={i.id}>
              <span>{i.product?.productName ?? 'Товар'}</span>
              <span className="muted">{i.quantity} шт. × {money(i.price)}</span>
            </li>
          ))}
        </ul>
      )}

      {onDelete && (
        <div className="order-actions">
          <button className="ghost danger-text" onClick={() => onDelete(order)}>
            Отменить заказ
          </button>
        </div>
      )}
    </article>
  )
}

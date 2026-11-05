export default function Toasts({ items, onClose }) {
  return (
    <div className="toasts" role="status" aria-live="polite">
      {items.map((t) => (
        <div key={t.id} className={`toast ${t.type}`}>
          <div className="toast-body">
            <div>{t.text}</div>
            {t.code && <div className="toast-code">{t.code}</div>}
          </div>
          <button className="toast-close" onClick={() => onClose(t.id)} aria-label="Закрыть">
            ×
          </button>
        </div>
      ))}
    </div>
  )
}

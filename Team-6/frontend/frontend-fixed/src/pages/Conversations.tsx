import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { apiFetch } from '../api/apiFetch'

interface Conversation {
  conversationId: string
  createDate: string
  status: string
  channel: string
  message: string
  workerId: string
}

export default function Conversations() {
  const [items, setItems] = useState<Conversation[]>([])
  const [error, setError] = useState('')
  const navigate = useNavigate()

  useEffect(() => {
    (async () => {
      try {
        const data = await apiFetch<Conversation[]>('/conversation/conversations', { method: 'GET' })
        setItems(data)
      } catch (e: any) {
        setError(e.message)
      }
    })()
  }, [])

  return (
    <div className="container">
      <div className="card" style={{ padding:16, marginTop:16 }}>
        <div style={{ display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:12 }}>
          <div style={{ fontSize:18, fontWeight:700 }}>Обращения</div>
        </div>
        {error && <div className="alert error">{error}</div>}
        <table className="table">
          <thead>
            <tr>
              <th>#</th>
              <th>Дата</th>
              <th>Канал</th>
              <th>Сообщение</th>
              <th>Статус</th>
            </tr>
          </thead>
          <tbody>
            {items.map((c) => (
              <tr key={c.conversationId} style={{ cursor:'pointer' }}
                  onClick={() => navigate(`/conversation?id=${encodeURIComponent(c.conversationId)}`)}>
                <td className="mono">#{c.conversationId.slice(0,8)}</td>
                <td>{new Date(c.createDate).toLocaleString()}</td>
                <td>{c.channel}</td>
                <td style={{ maxWidth:420, whiteSpace:'nowrap', overflow:'hidden', textOverflow:'ellipsis' }}>{c.message}</td>
                <td>{c.status}</td>
              </tr>
            ))}
            {items.length === 0 && (
              <tr><td colSpan={5} style={{ color:'#6b7280' }}>Нет записей</td></tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

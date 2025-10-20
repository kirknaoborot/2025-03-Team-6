import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { loadAuth } from '../auth/tokenStore'

type Stats = {
  total: number
  answered: number
  inWork: number
  withoutAnswer: number
}

type DailyPoint = { date: string; total: number }

export default function Statistics() {
  const { accessToken, user } = loadAuth()
  const navigate = useNavigate()
  const [stats, setStats] = useState<Stats | null>(null)
  const [daily, setDaily] = useState<DailyPoint[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    async function fetchStats() {
      try {
        const baseUrl =
          (import.meta as any).env?.VITE_API_URL ?? 'http://localhost:56466'
        const resp = await fetch(`${baseUrl}/conversation/statistics`, {
          headers: {
            'Accept': 'application/json',
            'Authorization': `Bearer ${accessToken}`
          }
        })
        if (!resp.ok) {
          const text = await resp.text()
          throw new Error(`HTTP ${resp.status}: ${text.slice(0,200)}`)
        }
        const ct = resp.headers.get('content-type') || ''
        if (!ct.toLowerCase().includes('application/json')) {
          const text = await resp.text()
          throw new Error(`Unexpected response type: ${ct}. Body: ${text.slice(0,200)}`)
        }
        const data = await resp.json()
        setStats(data)
      } catch (e: any) {
        setError(e.message || 'Error')
      }
    }
    fetchStats()
  }, [accessToken])

  useEffect(() => {
    async function fetchDaily() {
      try {
        const baseUrl = (import.meta as any).env?.VITE_API_URL ?? 'http://localhost:56466'
        const today = new Date()
        const from = new Date(today)
        from.setDate(today.getDate() - 14)
        const fmt = (d: Date) => d.toISOString().slice(0, 10)
        const url = `${baseUrl}/conversation/statistics/daily?from=${fmt(from)}&to=${fmt(today)}`
        const resp = await fetch(url, { headers: { 'Accept': 'application/json', 'Authorization': `Bearer ${accessToken}` } })
        if (!resp.ok) throw new Error(`HTTP ${resp.status}`)
        const list: { date: string; total: number }[] = await resp.json()
        // заполним пропуски нулями для ровной линии
        const map = new Map(list.map(x => [x.date, x.total]))
        const points: DailyPoint[] = []
        for (let d = new Date(from); d <= today; d.setDate(d.getDate() + 1)) {
          const key = d.toISOString().slice(0, 10)
          points.push({ date: key, total: map.get(key) ?? 0 })
        }
        setDaily(points)
      } catch (e: any) {
        // не блокируем страницу, просто показываем без графика
        console.warn('daily stats error', e)
      }
    }
    fetchDaily()
  }, [accessToken])

  const isAdmin = useMemo(() => {
    const role = user?.role
    if (role === undefined || role === null) return false
    const roleStr = String(role).toLowerCase()
    return roleStr === '0' || roleStr === 'administrator' || roleStr === 'администратор'
  }, [user])

  if (!isAdmin) return <div className="p-4">Доступ запрещён</div>

  return (
    <div className="p-4" style={{ display: 'grid', gap: 16, maxWidth: 1120, margin: '0 auto' }}>
      <div className="header" style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <div className="title" style={{ fontSize: 18, fontWeight: 600, color: '#111827', letterSpacing: '.2px' }}>Отчетность</div>
        <div className="actionbar" style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
          <button
            type="button"
            onClick={() => navigate('/')}
            style={{ border: '1px solid #d1d5db', background: '#fff', color: '#111827', borderRadius: 8, padding: '10px 14px', fontWeight: 600, cursor: 'pointer' }}
          >
            ← Назад
          </button>
        </div>
      </div>
      {error && <div className="text-red-600">{error}</div>}
      {!stats && !error && <div>Загрузка...</div>}
      {stats && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(1, minmax(0, 1fr))', gap: 12 }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(1, minmax(0, 1fr))', gap: 12 }} className="sm:grid-cols-2 lg:grid-cols-4">
            {[
              { label: 'Всего обращений', value: stats.total, color: '#3b82f6' },
              { label: 'Отвечено', value: stats.answered, color: '#10b981' },
              { label: 'В работе', value: stats.inWork, color: '#f59e0b' },
              { label: 'Без ответа', value: stats.withoutAnswer, color: '#f43f5e' },
            ].map((t, i) => (
              <div key={i} style={{
                position: 'relative',
                border: '1px solid #e5e7eb',
                borderRadius: 10,
                background: '#ffffff',
                boxShadow: '0 1px 2px rgba(16,24,40,.04), 0 1px 3px rgba(16,24,40,.06)',
                padding: 14,
                display: 'flex',
                flexDirection: 'column'
              }}>
                <div style={{ position: 'absolute', inset: 0, pointerEvents: 'none', borderTopLeftRadius: 12, borderTopRightRadius: 12 }} />
                <div style={{
                  position: 'absolute', left: 0, top: 0, bottom: 0,
                  width: 5, borderTopLeftRadius: 10, borderBottomLeftRadius: 10, background: t.color
                }} />
                <div style={{ color: t.color, fontSize: 12, fontWeight: 600 }}>{t.label}</div>
                <div style={{ marginTop: 6, fontSize: 28, fontWeight: 800, color: '#111827', letterSpacing: '-0.02em' }}>{t.value}</div>
              </div>
            ))}
          </div>
        </div>
      )}

      {daily.length > 0 && (
        <div className="card" style={{ border: '1px solid #e5e7eb', borderRadius: 10, background: '#fff', boxShadow: '0 1px 2px rgba(16,24,40,.04), 0 1px 3px rgba(16,24,40,.06)', padding: 14 }}>
          <div style={{ color: '#374151', fontWeight: 600, marginBottom: 8, fontSize: 14 }}>Поступления по дням (последние 14 дней)</div>
          {/* Простой SVG-график */}
          <Chart data={daily} />
        </div>
      )}
    </div>
  )
}

function Chart({ data }: { data: DailyPoint[] }) {
  const width = 800
  const height = 240
  const padding = 32
  const maxY = Math.max(1, ...data.map(d => d.total))
  const xStep = (width - padding * 2) / Math.max(1, data.length - 1)
  const y = (v: number) => height - padding - (v / maxY) * (height - padding * 2)
  const x = (i: number) => padding + i * xStep
  const d = data.map((p, i) => `${i === 0 ? 'M' : 'L'} ${x(i)} ${y(p.total)}`).join(' ')
  const labels = data.map((p, i) => ({ x: x(i), y: y(p.total), date: p.date.slice(5) }))
  return (
    <div>
      {/* Легенда */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 8 }}>
        <span style={{ width: 18, height: 3, background: '#2563eb', display: 'inline-block', borderRadius: 2 }} />
        <span style={{ fontSize: 12, color: '#374151' }}>Поступления в день</span>
      </div>
      <svg viewBox={`0 0 ${width} ${height}`} className="w-full h-auto">
        {/* оси */}
        <line x1={padding} y1={height - padding} x2={width - padding} y2={height - padding} stroke="#e5e7eb" />
        <line x1={padding} y1={padding} x2={padding} y2={height - padding} stroke="#e5e7eb" />
        {/* линия */}
        <path d={d} fill="none" stroke="#2563eb" strokeWidth={2} />
        {/* точки */}
        {labels.map((p, i) => (
          <g key={i}>
            <circle cx={p.x} cy={p.y} r={3} fill="#2563eb" />
          </g>
        ))}
        {/* подписи по X через день */}
        {labels.map((p, i) => i % 2 === 0 ? (
          <text key={"t"+i} x={p.x} y={height - padding + 16} fontSize={10} textAnchor="middle" fill="#6b7280">{p.date}</text>
        ) : null)}
        {/* подпись по Y макс */}
        <text x={padding - 8} y={padding} fontSize={10} textAnchor="end" fill="#6b7280">{maxY}</text>
        {/* подписи осей */}
        <text x={width - padding} y={height - padding + 28} fontSize={11} textAnchor="end" fill="#6b7280">Дата</text>
        <text x={padding - 32} y={height / 2} fontSize={11} textAnchor="middle" fill="#6b7280" transform={`rotate(-90 ${padding - 32},${height / 2})`}>
          Количество
        </text>
      </svg>
    </div>
  )
}



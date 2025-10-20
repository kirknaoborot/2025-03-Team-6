import { useEffect, useMemo, useState } from 'react'
import { loadAuth } from '../auth/tokenStore'

type Stats = {
  total: number
  answered: number
  inWork: number
  withoutAnswer: number
}

export default function Statistics() {
  const { accessToken, user } = loadAuth()
  const [stats, setStats] = useState<Stats | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    async function fetchStats() {
      try {
        const resp = await fetch('/conversation/statistics', {
          headers: {
            'Authorization': `Bearer ${accessToken}`
          }
        })
        if (!resp.ok) throw new Error('Failed to load statistics')
        const data = await resp.json()
        setStats(data)
      } catch (e: any) {
        setError(e.message || 'Error')
      }
    }
    fetchStats()
  }, [accessToken])

  const isAdmin = useMemo(() => {
    const role = user?.role
    if (role === undefined || role === null) return false
    const roleStr = String(role).toLowerCase()
    return roleStr === '0' || roleStr === 'administrator' || roleStr === 'администратор'
  }, [user])

  if (!isAdmin) return <div className="p-4">Доступ запрещён</div>

  return (
    <div className="p-4 space-y-4">
      <h1 className="text-2xl font-semibold">Отчетность</h1>
      {error && <div className="text-red-600">{error}</div>}
      {!stats && !error && <div>Загрузка...</div>}
      {stats && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="rounded border p-4">
            <div className="text-gray-500">Всего обращений</div>
            <div className="text-3xl font-bold">{stats.total}</div>
          </div>
          <div className="rounded border p-4">
            <div className="text-gray-500">Отвечено</div>
            <div className="text-3xl font-bold">{stats.answered}</div>
          </div>
          <div className="rounded border p-4">
            <div className="text-gray-500">В работе</div>
            <div className="text-3xl font-bold">{stats.inWork}</div>
          </div>
          <div className="rounded border p-4">
            <div className="text-gray-500">Без ответа</div>
            <div className="text-3xl font-bold">{stats.withoutAnswer}</div>
          </div>
        </div>
      )}
    </div>
  )
}



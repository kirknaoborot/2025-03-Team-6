import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { login as loginApi } from '../auth/authApi'

export default function Login() {
  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const navigate = useNavigate()

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!login || !password) { setError('Укажите логин и пароль'); return }
    setLoading(true); setError('')
    try {
      await loginApi(login, password)
      navigate('/')
    } catch (e: any) {
      setError(e?.message || 'Ошибка входа')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="container" style={{ display:'grid', placeItems:'center', minHeight:'100vh' }}>
      <style>{`
        .login-card { width: 100%; max-width: 420px; }
        .header { text-align:center; padding: 18px 18px 0 18px; }
        .title { font-size: 20px; font-weight: 700; color:#111827; }
        .subtitle { color:#6b7280; font-size:13px; margin-top:6px; }
        .body { padding: 18px; }
        .row { margin-bottom: 12px; }
        .actions { display:flex; gap: 10px; align-items:center; justify-content: space-between; margin-top: 6px; }
      `}</style>

      <div className="card login-card">
        <div className="header">
          <div className="title">Вход в систему</div>
          <div className="subtitle">Введите учётные данные</div>
        </div>
        <form className="body" onSubmit={onSubmit}>
          <div className="row">
            <div className="label">Логин</div>
            <input className="input" value={login} onChange={e => setLogin(e.target.value)} placeholder="example@company.com" />
          </div>
          <div className="row">
            <div className="label">Пароль</div>
            <input className="input" type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="••••••••" />
          </div>

          {error && <div className="alert error">{error}</div>}

          <div className="actions">
            <button className="btn primary" type="submit" disabled={loading}>
              {loading ? 'Входим…' : 'Войти'}
            </button>
            <a className="link" href="#" onClick={e => { e.preventDefault(); setLogin(''); setPassword(''); }}>
              Сбросить
            </a>
          </div>
        </form>
      </div>
    </div>
  )
}

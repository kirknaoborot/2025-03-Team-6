import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { loginUser } from '../services/authService';

export default function Login() {
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const response = await loginUser({ login, password });
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('user', JSON.stringify(response.user));
      navigate('/conversations');
    } catch (err: any) {
      setError(err.message || 'Ошибка входа');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container d-flex vh-100 justify-content-center align-items-center">
      <div className="card shadow p-4 text-center" style={{ maxWidth: 400, width: '100%' }}>
        
        {/* Центрированный логотип */}
        <div className="d-flex justify-content-center mb-3">
          <img
            src="/assets/sila-otchima.svg"
            alt="Сила Отчима"
            style={{ width: 100, height: 'auto' }}
          />
        </div>

        <h2 className="mb-3">Авторизация</h2>

        {error && (
          <div className="alert alert-danger text-center py-2">{error}</div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="mb-3 text-start">
            <label className="form-label">Логин</label>
            <input
              type="text"
              className="form-control"
              value={login}
              onChange={(e) => setLogin(e.target.value)}
              required
            />
          </div>
          <div className="mb-3 text-start">
            <label className="form-label">Пароль</label>
            <input
              type="password"
              className="form-control"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          <button
            type="submit"
            className="btn btn-dark w-100"
            disabled={loading}
          >
            {loading ? 'Вход...' : 'Войти'}
          </button>
        </form>
      </div>
    </div>
  );
}

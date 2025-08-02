import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface Conversation {
  id: string;
  createdAt: string;
  status: string;
}

export default function Conversations() {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchConversations = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const res = await fetch('http://localhost:5325/conversations', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) throw new Error('Ошибка при получении данных');

        const data = await res.json();
        setConversations(data);
      } catch (err: any) {
        setError(err.message || 'Ошибка');
      } finally {
        setLoading(false);
      }
    };

    fetchConversations();
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
    navigate('/');
  };

  const renderStatusBadge = (status: string) => {
    let color = 'secondary';

    if (status.toLowerCase().includes('завершено')) color = 'success';
    else if (status.toLowerCase().includes('в работе')) color = 'warning';
    else if (status.toLowerCase().includes('ошибка')) color = 'danger';

    return <span className={`badge bg-${color}`}>{status}</span>;
  };

  return (
    <div className="container mt-5">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="bi bi-chat-left-dots me-2"></i>
          Обращения
        </h2>
        <button className="btn btn-outline-danger" onClick={handleLogout}>
          Выйти
        </button>
      </div>

      {loading && <div className="alert alert-info">Загрузка...</div>}
      {error && <div className="alert alert-danger">{error}</div>}

      {!loading && !error && (
        <div className="table-responsive">
          <table className="table table-bordered table-hover align-middle">
            <thead className="table-light">
              <tr>
                <th>Номер обращения</th>
                <th>Создано</th>
                <th>Статус</th>
              </tr>
            </thead>
            <tbody>
              {conversations.map((conv) => (
                <tr key={conv.id}>
                  <td>{conv.id}</td>
                  <td>{new Date(conv.createdAt).toLocaleString()}</td>
                  <td>{renderStatusBadge(conv.status)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

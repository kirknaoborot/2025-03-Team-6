import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import * as signalR from "@microsoft/signalr";

interface Conversation {
  conversationId: string;
  createDate: string;
  status: string;
  channel: string;
  message: string;
  workerId: string;
}

export default function Conversations() {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [onlineStatus, setOnlineStatus] = useState(false); // <-- новый стейт для индикатора
  const navigate = useNavigate();

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:54000/onlinestatus", { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => {
        console.log("Connected to SignalR Hub");

        // Отправляем серверу, что пользователь онлайн
        const user = localStorage.getItem("user") || "anonymous";
        connection.invoke("UserOnline", user);

        // Устанавливаем флаг, чтобы показать плашку
        setOnlineStatus(true);
      })
      .catch(err => console.error(err));

    return () => {
      connection.stop();
    };
  }, []);

  useEffect(() => {
    const fetchConversations = async () => {
      try {
        const token = localStorage.getItem("accessToken");
        const res = await fetch("http://localhost:56466/conversation/conversations", {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Ошибка при получении данных");
        const data = await res.json();
        setConversations(data);
      } catch (err: any) {
        setError(err.message || "Ошибка");
      } finally {
        setLoading(false);
      }
    };

    fetchConversations();
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("user");
    navigate("/");
  };

  const renderStatusBadge = (status: string) => {
    let color = "secondary";
    if (status.toLowerCase().includes("завершено")) color = "success";
    else if (status.toLowerCase().includes("в работе")) color = "warning";
    else if (status.toLowerCase().includes("ошибка")) color = "danger";
    return <span className={`badge bg-${color}`}>{status}</span>;
  };

  return (
    <div className="container mt-5">
      {/* Индикатор онлайн-статуса */}
      {onlineStatus && (
        <div className="alert alert-success">
          Вы перешли в статус <strong>готов</strong>
        </div>
      )}

      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Обращения</h2>
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
                <th>ID</th>
                <th>Дата создания</th>
                <th>Канал</th>
                <th>Сообщение</th>
                <th>Статус</th>
              </tr>
            </thead>
            <tbody>
              {conversations.map(conv => (
                <tr key={conv.conversationId}>
                  <td>{conv.conversationId}</td>
                  <td>{new Date(conv.createDate).toLocaleString()}</td>
                  <td>{conv.channel}</td>
                  <td>{conv.message}</td>
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

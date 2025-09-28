import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import * as signalR from "@microsoft/signalr";

interface Conversation {
  conversationId: string;
  createDate: string;
  status: string; // теперь это строка: "New", "InWork", ...
  channel: string;
  message: string;
  workerId: string;
}

// Подписи к статусам
const statusLabels: Record<string, string> = {
  New: "Новое",
  Distributed: "Распределено",
  InWork: "В работе",
  Closed: "Обработано",
  AgentNotFound: "Агент не был найден",
};

// Цвета бейджей
const statusColors: Record<string, string> = {
  New: "primary",
  Distributed: "info",
  InWork: "warning",
  Closed: "success",
  AgentNotFound: "danger",
};

export default function Conversations() {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [onlineStatus, setOnlineStatus] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [activeTab, setActiveTab] = useState<
    "all" | "new" | "inWork" | "closed" | "agentNotFound" | "distributed"
  >("all");

  const navigate = useNavigate();

// SignalR подключение
useEffect(() => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:54000/onlinestatus", { withCredentials: true })
    .withAutomaticReconnect()
    .build();

  connection
    .start()
    .then(() => {
      console.log("Connected to SignalR Hub");

      const storedUser = localStorage.getItem("user");
      let userId: string = "anonymous";

      if (storedUser) {
        try {
          const parsed = JSON.parse(storedUser);
          userId = parsed.id?.toString() || "anonymous";
        } catch (e) {
          console.error("Ошибка парсинга user из localStorage", e);
        }
      }

      connection.invoke("UserOnline", userId);

      setOnlineStatus(true);
      setShowModal(true); // показываем модалку при подключении
    })
    .catch((err) => console.error(err));

  return () => connection.stop();
}, []);

  // Загрузка обращений
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
    return (
      <span className={`badge bg-${statusColors[status] || "secondary"}`}>
        {statusLabels[status] || status}
      </span>
    );
  };

  // Подсчёты для вкладок
  const counts = {
    all: conversations.length,
    new: conversations.filter((c) => c.status === "New").length,
    inWork: conversations.filter((c) => c.status === "InWork").length,
    closed: conversations.filter((c) => c.status === "Closed").length,
    distributed: conversations.filter((c) => c.status === "Distributed").length,
    agentNotFound: conversations.filter((c) => c.status === "AgentNotFound").length,
  };

  // Фильтрация по активной вкладке
  const filteredConversations = conversations.filter((c) => {
    switch (activeTab) {
      case "new":
        return c.status === "New";
      case "inWork":
        return c.status === "InWork";
      case "closed":
        return c.status === "Closed";
      case "distributed":
        return c.status === "Distributed";
      case "agentNotFound":
        return c.status === "AgentNotFound";
      default:
        return true;
    }
  });

  return (
    <div className="container mt-5">
      {onlineStatus && (
        <div className="alert alert-success">
          Вы перешли в статус <strong>готов</strong>
        </div>
      )}

      {/* Модальное окно */}
      {showModal && (
        <div
          className="modal show fade"
          style={{ display: "block", backgroundColor: "rgba(0,0,0,0.5)" }}
          tabIndex={-1}
        >
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Статус: готов</h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowModal(false)}
                ></button>
              </div>
              <div className="modal-body">
                <p>
                  Вы переведены в статус <strong>готов</strong>, на вас будут распределяться
                  обращения.
                </p>
              </div>
              <div className="modal-footer">
                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={() => setShowModal(false)}
                >
                  Понятно
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Обращения</h2>
        <button className="btn btn-outline-danger" onClick={handleLogout}>
          Выйти
        </button>
      </div>

      {/* Вкладки */}
      <ul className="nav nav-tabs mb-3">
        <li className="nav-item">
          <button
            className={`nav-link ${activeTab === "all" ? "active" : ""}`}
            onClick={() => setActiveTab("all")}
          >
            Все <span className="badge bg-secondary">{counts.all}</span>
          </button>
        </li>
        <li className="nav-item">
          <button
            className={`nav-link ${activeTab === "new" ? "active" : ""}`}
            onClick={() => setActiveTab("new")}
          >
            Новые <span className="badge bg-primary">{counts.new}</span>
          </button>
        </li>
        <li className="nav-item">
          <button
            className={`nav-link ${activeTab === "inWork" ? "active" : ""}`}
            onClick={() => setActiveTab("inWork")}
          >
            В работе{" "}
            <span className="badge bg-warning text-dark">{counts.inWork}</span>
          </button>
        </li>
        <li className="nav-item">
          <button
            className={`nav-link ${activeTab === "closed" ? "active" : ""}`}
            onClick={() => setActiveTab("closed")}
          >
            Завершенные <span className="badge bg-success">{counts.closed}</span>
          </button>
        </li>
        <li className="nav-item">
          <button
            className={`nav-link ${activeTab === "distributed" ? "active" : ""}`}
            onClick={() => setActiveTab("distributed")}
          >
            Распределено <span className="badge bg-info">{counts.distributed}</span>
          </button>
        </li>
        <li className="nav-item">
          <button
            className={`nav-link ${activeTab === "agentNotFound" ? "active" : ""}`}
            onClick={() => setActiveTab("agentNotFound")}
          >
            Без агента{" "}
            <span className="badge bg-danger">{counts.agentNotFound}</span>
          </button>
        </li>
      </ul>

      {loading && <div className="alert alert-info">Загрузка...</div>}
      {error && <div className="alert alert-danger">{error}</div>}

      {!loading && !error && (
        <div className="table-responsive">
          <table className="table table-bordered table-hover align-middle">
            <thead className="table-light">
              <tr>
                <th>#</th>
                <th>Дата создания</th>
                <th>Канал</th>
                <th>Сообщение</th>
                <th>Статус</th>
              </tr>
            </thead>
            <tbody>
              {filteredConversations.map((conv) => (
                <tr
                  key={conv.conversationId}
                  style={{ cursor: "pointer" }}
                  onClick={() => navigate(`/conversation/${conv.conversationId}`)}
                >
                  <td>#{conv.conversationId.slice(0, 8)}</td>
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

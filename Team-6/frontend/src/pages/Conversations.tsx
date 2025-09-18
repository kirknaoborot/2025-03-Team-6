import { useEffect, useMemo, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import * as signalR from "@microsoft/signalr";
import { apiFetch } from "../api/apiFetch";

interface Conversation {
  conversationId: string;
  createDate: string;
  status: StatusKey;
  channel: string;
  message: string;
  workerId: string;
}

type TabKey = "all" | "new" | "inWork" | "closed" | "agentNotFound" | "distributed";
type StatusKey = "New" | "Distributed" | "InWork" | "Closed" | "AgentNotFound";

const statusLabels: Record<StatusKey, string> = {
  New: "Новое",
  Distributed: "Распределено",
  InWork: "В работе",
  Closed: "Обработано",
  AgentNotFound: "Агент не найден",
};

const statusDots: Record<StatusKey, string> = {
  New: "#6b7280",
  Distributed: "#0d6efd",
  InWork: "#b08900",
  Closed: "#198754",
  AgentNotFound: "#dc3545",
};

export default function Conversations() {
  const navigate = useNavigate();

  // data
  const [items, setItems] = useState<Conversation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // filters
  const [activeTab, setActiveTab] = useState<TabKey>("all");
  const [search, setSearch] = useState("");

  // signalR status toggle
  const [connected, setConnected] = useState(false);
  const [connecting, setConnecting] = useState(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const userId = useMemo(() => (localStorage.getItem("user") || "anonymous"), []);

  // styles (строгий минимализм)
  const styles = `
    .layout { display:grid; grid-template-columns: 260px 1fr; gap:24px; }
    @media (max-width: 992px){ .layout { grid-template-columns:1fr; } }
    .card { border:1px solid #e5e7eb; border-radius:10px; background:#fff; }
    .section { padding:16px 18px; }
    .section + .section { border-top:1px solid #f3f4f6; }
    .header { display:flex; align-items:center; justify-content:space-between; margin-bottom:18px; }
    .title { font-size:18px; font-weight:600; color:#111827; letter-spacing:.2px; }
    .muted { color:#6b7280; font-size:13px; }
    .segmented { display:inline-flex; border:1px solid #d1d5db; border-radius:8px; overflow:hidden; }
    .segmented button { appearance:none; background:#fff; border:none; padding:8px 14px; font-weight:600; font-size:14px; color:#374151; cursor:pointer; min-width:110px; }
    .segmented button + button { border-left:1px solid #e5e7eb; }
    .segmented .active { background:#111827; color:#fff; }
    .sidebar .section-title { font-size:12px; color:#6b7280; text-transform:uppercase; letter-spacing:.08em; padding:12px 14px; border-bottom:1px solid #f3f4f6; }
    .nav-vert { padding: 8px; display:flex; flex-direction:column; gap:6px; }
    .nav-item { display:flex; justify-content:space-between; align-items:center; padding:10px 12px; border-radius:8px; color:#374151; cursor:pointer; }
    .nav-item:hover { background:#f9fafb; }
    .nav-item.active { background:#eef2ff; color:#111827; border:1px solid #e5e7eb; }
    .nav-count { font-size:12px; color:#6b7280; }
    .search { display:flex; gap:8px; padding:10px; }
    .input { width:100%; border:1px solid #e5e7eb; border-radius:8px; padding:8px 10px; font-size:14px; }
    .table-wrap { overflow:auto; }
    table { width:100%; border-collapse:collapse; }
    thead th { position:sticky; top:0; background:#f9fafb; color:#374151; font-weight:600; font-size:13px; letter-spacing:.02em; border-bottom:1px solid #e5e7eb; padding:10px 12px; text-align:left; }
    tbody td { padding:12px; border-bottom:1px solid #f3f4f6; font-size:14px; color:#111827; }
    tbody tr:hover { background:#fcfcfd; }
    .mono { font-family: ui-monospace, SFMono-Regular, Menlo, monospace; }
    .status { display:inline-flex; align-items:center; gap:8px; font-size:13px; color:#374151; }
    .dot { width:8px; height:8px; border-radius:50%; display:inline-block; }
    .btn { border:1px solid #d1d5db; background:#111827; color:#fff; border-radius:8px; padding:10px 14px; font-weight:600; }
    .btn.secondary { background:#fff; color:#111827; }
    .helpbar { margin: 6px 0 16px; color:#6b7280; font-size:13px; }
  `;

  // load data
  useEffect(() => {
    (async () => {
      setLoading(true);
      setError("");
      try {
        const data = await apiFetch<Conversation[]>("/conversation/conversations", { method: "GET" });
        setItems(data);
      } catch (e: any) {
        setError(e.message || "Ошибка получения данных");
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  // signalR helpers
  const ensureConnection = () => {
    if (!connectionRef.current) {
      const c = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:54000/onlinestatus", { withCredentials: true })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      c.on("UserCameOnline", (uid) => console.log("UserCameOnline:", uid));
      c.onreconnected(() => setConnected(true));
      c.onclose(() => setConnected(false));
      connectionRef.current = c;
    }
    return connectionRef.current!;
  };

  const connect = async () => {
    if (connecting || connected) return;
    setConnecting(true);
    try {
      const conn = ensureConnection();
      await conn.start();
      await conn.invoke("UserOnline", userId);
      setConnected(true);
    } catch (e) {
      console.error("SignalR connect error:", e);
      setConnected(false);
    } finally {
      setConnecting(false);
    }
  };

  const disconnect = async () => {
    if (connecting) return;
    try {
      await connectionRef.current?.stop();
    } catch {}
    setConnected(false);
  };

  useEffect(() => {
    return () => {
      connectionRef.current?.stop().catch(() => {});
      connectionRef.current = null;
    };
  }, []);

  // counts & filtered
  const counts = useMemo(() => ({
    all: items.length,
    new: items.filter((c) => c.status === "New").length,
    inWork: items.filter((c) => c.status === "InWork").length,
    closed: items.filter((c) => c.status === "Closed").length,
    distributed: items.filter((c) => c.status === "Distributed").length,
    agentNotFound: items.filter((c) => c.status === "AgentNotFound").length,
  }), [items]);

  const filtered = useMemo(() => {
    let list = items;
    // фильтр по табу
    switch (activeTab) {
      case "new": list = list.filter((c) => c.status === "New"); break;
      case "inWork": list = list.filter((c) => c.status === "InWork"); break;
      case "closed": list = list.filter((c) => c.status === "Closed"); break;
      case "distributed": list = list.filter((c) => c.status === "Distributed"); break;
      case "agentNotFound": list = list.filter((c) => c.status === "AgentNotFound"); break;
      default: break;
    }
    // текстовый поиск (по сообщению и каналу)
    if (search.trim()) {
      const q = search.trim().toLowerCase();
      list = list.filter((c) =>
        (c.message || "").toLowerCase().includes(q) ||
        (c.channel || "").toLowerCase().includes(q) ||
        c.conversationId.toLowerCase().includes(q)
      );
    }
    return list;
  }, [items, activeTab, search]);

  const goDetail = (id: string) => navigate(`/conversation?id=${encodeURIComponent(id)}`);

  return (
    <div className="container py-4">
      <style>{styles}</style>

      {/* Шапка + переключатель статуса */}
      <div className="header">
        <div>
          <div className="title">Обращения</div>
          <div className="muted">
            {connected ? "Статус: готов — распределение включено" : "Статус: неготов — распределение выключено"}
          </div>
        </div>
        <div className="segmented" aria-label="Статус оператора">
          <button
            className={!connected ? "active" : ""}
            onClick={disconnect}
            disabled={!connected || connecting}
            title="Сделать статус Неготов"
          >
            Не готов
          </button>
          <button
            className={connected ? "active" : ""}
            onClick={connect}
            disabled={connected || connecting}
            title="Сделать статус Готов"
          >
            Готов
          </button>
        </div>
      </div>

      <div className="helpbar">
        {connecting
          ? "Выполняется переключение статуса…"
          : connected
          ? "Вы получаете новые обращения. Переключите на «Неготов», чтобы остановить распределение."
          : "Нажмите «Готов», чтобы включить распределение обращений."}
      </div>

      <div className="layout">
        {/* LEFT: фильтры */}
        <aside className="sidebar">
          <div className="card">
            <div className="section-title">Фильтр</div>

            <div className="search">
              <input
                className="input"
                placeholder="Поиск по сообщению, каналу, ID…"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              />
            </div>

            <div className="nav-vert">
              <div className={`nav-item ${activeTab === "all" ? "active" : ""}`} onClick={() => setActiveTab("all")}>
                <span>Все</span><span className="nav-count">{counts.all}</span>
              </div>
              <div className={`nav-item ${activeTab === "new" ? "active" : ""}`} onClick={() => setActiveTab("new")}>
                <span>Новые</span><span className="nav-count">{counts.new}</span>
              </div>
              <div className={`nav-item ${activeTab === "inWork" ? "active" : ""}`} onClick={() => setActiveTab("inWork")}>
                <span>В работе</span><span className="nav-count">{counts.inWork}</span>
              </div>
              <div className={`nav-item ${activeTab === "distributed" ? "active" : ""}`} onClick={() => setActiveTab("distributed")}>
                <span>Распределено</span><span className="nav-count">{counts.distributed}</span>
              </div>
              <div className={`nav-item ${activeTab === "closed" ? "active" : ""}`} onClick={() => setActiveTab("closed")}>
                <span>Завершённые</span><span className="nav-count">{counts.closed}</span>
              </div>
              <div className={`nav-item ${activeTab === "agentNotFound" ? "active" : ""}`} onClick={() => setActiveTab("agentNotFound")}>
                <span>Без агента</span><span className="nav-count">{counts.agentNotFound}</span>
              </div>
            </div>
          </div>
        </aside>

        {/* RIGHT: таблица */}
        <section>
          <div className="card">
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th style={{ width: "10rem" }}>№ обращения</th>
                    <th style={{ width: "16rem" }}>Дата создания</th>
                    <th style={{ width: "10rem" }}>Канал</th>
                    <th>Сообщение</th>
                    <th style={{ width: "12rem" }}>Статус</th>
                  </tr>
                </thead>
                <tbody>
                  {loading && (
                    <tr><td colSpan={5}>Загрузка…</td></tr>
                  )}
                  {error && !loading && (
                    <tr><td colSpan={5} style={{ color: "#b91c1c" }}>{error}</td></tr>
                  )}
                  {!loading && !error && filtered.map((c) => (
                    <tr key={c.conversationId} style={{ cursor: "pointer" }} onClick={() => goDetail(c.conversationId)}>
                      <td className="mono">{c.number}</td>
                      <td>{new Date(c.createDate).toLocaleString()}</td>
                      <td>{c.channel}</td>
                      <td title={c.message} style={{ maxWidth: 520, whiteSpace: "nowrap", overflow: "hidden", textOverflow: "ellipsis" }}>
                        {c.message}
                      </td>
                      <td>
                        <span className="status">
                          <span className="dot" style={{ backgroundColor: statusDots[c.status] || "#6b7280" }} />
                          {statusLabels[c.status] || c.status}
                        </span>
                      </td>
                    </tr>
                  ))}
                  {!loading && !error && filtered.length === 0 && (
                    <tr><td colSpan={5} style={{ color: "#6b7280", textAlign: "center", padding: "24px" }}>
                      Нет записей по выбранному фильтру
                    </td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
}
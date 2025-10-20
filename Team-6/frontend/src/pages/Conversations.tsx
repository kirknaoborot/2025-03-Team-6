import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSignalR } from "../signalr/SignalRProvider";

/* ===== Types ===== */
interface Conversation {
  conversationId: string;
  createDate: string;
  status: StatusKey;
  channel: string;
  message: string;
  workerId: string;
  number: string;
}
type StatusKey = "New" | "Distributed" | "InWork" | "Closed" | "AgentNotFound";
type TabKey =
  | "all"
  | "new"
  | "inWork"
  | "closed"
  | "agentNotFound"
  | "distributed";

interface User {
  id: string;
  fullName: string;
  login: string;
  passwordsHash?: string;
  role: string | number;
  isActive: boolean;
}
type AuthStorage = {
  accessToken: string;
  refreshToken?: string;
  refreshTokenExpiryTime?: string;
  user: User;
};

/* ===== UI dicts ===== */
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

/* ===== Styles ===== */
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

.btn { border:1px solid #d1d5db; background:#111827; color:#fff; border-radius:8px; padding:10px 14px; font-weight:600; cursor:pointer; }
.btn.secondary { background:#fff; color:#111827; }
.btn:disabled { opacity:0.6; cursor:not-allowed; }
.helpbar { margin: 6px 0 16px; color:#6b7280; font-size:13px; }
.actionbar { display:flex; gap:8px; align-items:center; }

.pagination { display:flex; justify-content:space-between; align-items:center; padding:12px 16px; border-top:1px solid #f3f4f6; }
`;

export default function Conversations() {
  const navigate = useNavigate();
  const { connected, connecting, start, stop } = useSignalR();

  // данные обращений
  const [items, setItems] = useState<Conversation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // фильтры
  const [activeTab, setActiveTab] = useState<TabKey>("all");
  const [search, setSearch] = useState("");

  // пагинация
  const [page, setPage] = useState(1);
  const [perPage, setPerPage] = useState(10);

  /* ===== Auth ===== */
  const authObj = useMemo(() => {
    try {
      const raw = localStorage.getItem("auth");
      return raw ? (JSON.parse(raw) as AuthStorage) : null;
    } catch {
      return null;
    }
  }, []);

  const accessToken = authObj?.accessToken ?? null;
  const currentUser = authObj?.user ?? null;

  const userRole = currentUser?.role;
  const isAdmin = (() => {
    if (userRole === undefined || userRole === null) return false;
    const roleStr = String(userRole).toLowerCase();
    return (
      roleStr === "0" ||
      roleStr === "administrator" ||
      roleStr === "администратор"
    );
  })();

  /* ===== Load data ===== */
  useEffect(() => {
    (async () => {
      setLoading(true);
      setError("");
      try {
        const baseUrl =
          import.meta.env.VITE_API_URL ?? "http://localhost:56466";
        if (!accessToken) {
          setError("Нет accessToken (ключ 'auth' в localStorage пустой)");
          setLoading(false);
          return;
        }
        const res = await fetch(`${baseUrl}/conversation/conversations`, {
          headers: {
            Accept: "application/json",
            Authorization: `Bearer ${accessToken}`,
          },
        });

        if (res.status === 401) {
          localStorage.removeItem("auth");
          navigate("/login");
          return;
        }

        if (!res.ok) {
          const txt = await res.text();
          throw new Error(txt || `HTTP ${res.status}`);
        }

        const data: Conversation[] = await res.json();
        setItems(data);
      } catch (e: any) {
        setError(e?.message || "Ошибка получения данных");
      } finally {
        setLoading(false);
      }
    })();
  }, [accessToken, navigate]);

  /* ===== Обновление при SignalR событии ===== */
  useEffect(() => {
    const handler = async () => {
      console.log("🔁 Обновляем обращения после события ConversationDistributed");
      try {
        const baseUrl =
          import.meta.env.VITE_API_URL ?? "http://localhost:56466";
        const res = await fetch(`${baseUrl}/conversation/conversations`, {
          headers: {
            Accept: "application/json",
            Authorization: `Bearer ${accessToken}`,
          },
        });
        if (res.ok) {
          const data: Conversation[] = await res.json();
          setItems(data);
        }
      } catch (err) {
        console.error("Ошибка обновления обращений:", err);
      }
    };

    window.addEventListener("conversation:update", handler);
    return () => window.removeEventListener("conversation:update", handler);
  }, [accessToken]);

  /* ===== Derived data ===== */
  const counts = useMemo(
    () => ({
      all: items.length,
      new: items.filter((c) => c.status === "New").length,
      inWork: items.filter((c) => c.status === "InWork").length,
      closed: items.filter((c) => c.status === "Closed").length,
      distributed: items.filter((c) => c.status === "Distributed").length,
      agentNotFound: items.filter((c) => c.status === "AgentNotFound").length,
    }),
    [items]
  );

  const filtered = useMemo(() => {
    let list = items;
    switch (activeTab) {
      case "new":
        list = list.filter((c) => c.status === "New");
        break;
      case "inWork":
        list = list.filter((c) => c.status === "InWork");
        break;
      case "closed":
        list = list.filter((c) => c.status === "Closed");
        break;
      case "distributed":
        list = list.filter((c) => c.status === "Distributed");
        break;
      case "agentNotFound":
        list = list.filter((c) => c.status === "AgentNotFound");
        break;
      default:
        break;
    }
    if (search.trim()) {
      const q = search.trim().toLowerCase();
      list = list.filter(
        (c) =>
          (c.message || "").toLowerCase().includes(q) ||
          (c.channel || "").toLowerCase().includes(q) ||
          c.conversationId.toLowerCase().includes(q)
      );
    }
    return list;
  }, [items, activeTab, search]);

  // сбрасываем страницу при фильтре
  useEffect(() => {
    setPage(1);
  }, [activeTab, search]);

  const totalPages = Math.ceil(filtered.length / perPage);
  const paged = useMemo(() => {
    const start = (page - 1) * perPage;
    const end = start + perPage;
    return filtered.slice(start, end);
  }, [filtered, page, perPage]);

  const goDetail = (id: string) =>
    navigate(`/conversation?id=${encodeURIComponent(id)}`);

  /* ===== Logout ===== */
  const handleLogout = async (e?: React.MouseEvent) => {
    e?.preventDefault();
    try {
      await stop();
    } catch {}
    localStorage.removeItem("auth");
    navigate("/login", { replace: true });
  };

  /* ===== Render ===== */
  return (
    <div className="container py-4">
      <style>{styles}</style>

      <div className="header">
        <div>
          <div className="title">Обращения</div>
          <div className="muted">
            {connected
              ? "Статус: готов — распределение включено"
              : "Статус: неготов — распределение выключено"}
          </div>
        </div>
        <div className="actionbar">
          <div className="segmented" aria-label="Статус оператора">
            <button
              className={!connected ? "active" : ""}
              onClick={stop}
              disabled={!connected || connecting}
            >
              Не готов
            </button>
            <button
              className={connected ? "active" : ""}
              onClick={start}
              disabled={connected || connecting}
            >
              {connecting ? "Подключение…" : "Готов"}
            </button>
          </div>

          {isAdmin && (
            <>
              <button
                type="button"
                className="btn"
                onClick={() => navigate("/users/new")}
              >
                Пользователи
              </button>
              <button
                type="button"
                className="btn"
                onClick={() => navigate("/channels")}
              >
                Каналы
              </button>
              <button
                type="button"
                className="btn"
                onClick={() => navigate("/statistics")}
              >
                Отчетность
              </button>
            </>
          )}

          <button className="btn secondary" onClick={handleLogout}>
            Выйти
          </button>
        </div>
      </div>

      <div className="helpbar">
        {connecting
          ? "Выполняется переключение статуса…"
          : connected
          ? "Вы получаете новые обращения."
          : "Нажмите «Готов», чтобы включить распределение обращений."}
      </div>

      <div className="layout">
        {/* Sidebar */}
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
              {([["all", "Все"], ["new", "Новые"], ["inWork", "В работе"], ["distributed", "Распределено"], ["closed", "Завершённые"], ["agentNotFound", "Без агента"]] as [TabKey, string][]).map(([key, label]) => (
                <div
                  key={key}
                  className={`nav-item ${activeTab === key ? "active" : ""}`}
                  onClick={() => setActiveTab(key)}
                >
                  <span>{label}</span>
                  <span className="nav-count">{counts[key]}</span>
                </div>
              ))}
            </div>
          </div>
        </aside>

        {/* Table */}
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
                    <tr>
                      <td colSpan={5}>Загрузка…</td>
                    </tr>
                  )}
                  {error && !loading && (
                    <tr>
                      <td colSpan={5} style={{ color: "#b91c1c" }}>
                        {error}
                      </td>
                    </tr>
                  )}
                  {!loading &&
                    !error &&
                    paged.map((c) => (
                      <tr
                        key={c.conversationId}
                        style={{ cursor: "pointer" }}
                        onClick={() => goDetail(c.conversationId)}
                      >
                        <td className="mono">#{c.number}</td>
                        <td>{new Date(c.createDate).toLocaleString()}</td>
                        <td>{c.channel}</td>
                        <td
                          title={c.message}
                          style={{
                            maxWidth: 520,
                            whiteSpace: "nowrap",
                            overflow: "hidden",
                            textOverflow: "ellipsis",
                          }}
                        >
                          {c.message}
                        </td>
                        <td>
                          <span className="status">
                            <span
                              className="dot"
                              style={{
                                backgroundColor:
                                  statusDots[c.status] || "#6b7280",
                              }}
                            />
                            {statusLabels[c.status] || c.status}
                          </span>
                        </td>
                      </tr>
                    ))}
                  {!loading && !error && paged.length === 0 && (
                    <tr>
                      <td colSpan={5} style={{ color: "#6b7280", textAlign: "center", padding: "24px" }}>
                        Нет записей по выбранному фильтру
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="pagination">
              <div>
                Страница {page} из {totalPages || 1}
              </div>
              <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                <button
                  className="btn secondary"
                  disabled={page <= 1}
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                >
                  ← Назад
                </button>
                <button
                  className="btn secondary"
                  disabled={page >= totalPages}
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                >
                  Вперёд →
                </button>
                <select
                  className="input"
                  style={{ width: 80 }}
                  value={perPage}
                  onChange={(e) => {
                    setPerPage(Number(e.target.value));
                    setPage(1);
                  }}
                >
                  {[5, 10, 20, 50].map((n) => (
                    <option key={n} value={n}>
                      {n}/стр
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
}

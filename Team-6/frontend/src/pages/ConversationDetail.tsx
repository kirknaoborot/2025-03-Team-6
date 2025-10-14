import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { apiFetch } from "../api/apiFetch";

type TicketStatus = "New" | "Distributed" | "InWork" | "Closed" | "AgentNotFound";
type Priority = "Low" | "Normal" | "High" | "Urgent";

interface HistoryItem {
  id: number;
  author: "user" | "agent" | "system";
  text: string;
  createdAt: string;
}

interface TicketDetail {
  conversationId: string;
  subject: string;
  message: string;
  status: TicketStatus;
  priority: Priority;
  channel: string;
  tags: string[];
  createDate: string;
  lastUpdate: string;
  requester: { id?: string; name: string; email?: string; phone?: string };
  assignedAgent?: { id?: string; name?: string };
  history: HistoryItem[];
  number: string;
  answer?: string;
}

const statusLabels: Record<TicketStatus, string> = {
  New: "Новое",
  Distributed: "Распределено",
  InWork: "В работе",
  Closed: "Обработано",
  AgentNotFound: "Агент не найден",
};
const statusDots: Record<TicketStatus, string> = {
  New: "#6b7280",
  Distributed: "#0d6efd",
  InWork: "#b08900",
  Closed: "#198754",
  AgentNotFound: "#dc3545",
};

export default function ConversationDetail() {
  const [sp] = useSearchParams();
  const id = (sp.get("id") || "").trim();
  const navigate = useNavigate();

  const [ticket, setTicket] = useState<TicketDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [banner, setBanner] = useState<null | { type: "success" | "error"; text: string }>(null);
  const [error, setError] = useState<string | null>(null);

  const [resolution, setResolution] = useState("");
  const [sending, setSending] = useState(false);

  const styles = `
    .wrap { display:grid; grid-template-columns: 1.15fr 1fr; gap:24px; }
    @media (max-width: 992px){ .wrap{ grid-template-columns:1fr; } }
    .card { border:1px solid #e5e7eb; border-radius:10px; background:#fff; }
    .section { padding:16px 18px; }
    .section + .section { border-top:1px solid #f3f4f6; }
    .header { display:flex; align-items:center; justify-content:space-between; margin-bottom:18px; }
    .title { font-size:18px; font-weight:600; color:#111827; letter-spacing:.2px; }
    .muted { color:#6b7280; font-size:13px; }
    .mono { font-family: ui-monospace, SFMono-Regular, Menlo, monospace; }
    .grid { display:grid; grid-template-columns: repeat(2,minmax(0,1fr)); gap:12px 16px; }
    @media (max-width: 576px){ .grid{ grid-template-columns:1fr; } }
    .label { font-size:12px; color:#6b7280; text-transform:uppercase; letter-spacing:.08em; }
    .value { font-size:14px; color:#111827; }
    .pill { display:inline-block; padding:6px 10px; border:1px solid #e5e7eb; border-radius:999px; font-size:12px; color:#374151; background:#fafafa; }
    .tag { margin-right:6px; margin-bottom:6px; }
    .banner { padding:10px 12px; border-radius:8px; font-size:14px; margin-bottom:14px; }
    .banner.success { background:#ecfdf5; border:1px solid #a7f3d0; color:#065f46; }
    .banner.error { background:#fef2f2; border:1px solid #fecaca; color:#991b1b; }
    .dot { display:inline-block; width:8px; height:8px; border-radius:50%; margin-right:8px; }
    .desc { white-space: pre-wrap; }
    .timeline { position:relative; padding-left:16px; }
    .timeline::before { content:""; position:absolute; left:6px; top:0; bottom:0; width:2px; background:#f0f0f0; }
    .tl-item { position:relative; margin-bottom:12px; }
    .tl-dot { position:absolute; left:-1px; top:4px; width:10px; height:10px; border-radius:50%; background:#9ca3af; }
    .tl-head { font-weight:600; font-size:13px; color:#374151; }
    .tl-text { font-size:14px; color:#111827; }
    .editor textarea { width:100%; min-height:140px; resize:vertical; border:1px solid #e5e7eb; border-radius:8px; padding:10px 12px; font-size:14px; color:#111827; }
    .btns { display:flex; gap:10px; flex-wrap:wrap; }
    .btn { border:1px solid #d1d5db; background:#111827; color:#fff; border-radius:8px; padding:10px 14px; font-weight:600; }
    .btn.secondary { background:#fff; color:#111827; }
    .btn:disabled { opacity:.6; cursor:not-allowed; }
  `;

  // --- загрузка обращения
  useEffect(() => {
    let cancelled = false;
    (async () => {
      setLoading(true);
      setError(null);
      setBanner(null);

      if (!id) {
        setError("Не указан идентификатор обращения (?id=)");
        setLoading(false);
        return;
      }

      try {
        const data = await apiFetch<any>(`/conversation/conversation?id=${encodeURIComponent(id)}`);

        const detail: TicketDetail = {
          conversationId: data.conversationId ?? id,
          number: data.number ?? `O-${id.slice(0, 8)}`,
          subject: data.subject ?? "Обращение пользователя",
          message: data.message ?? "",
          status: (data.status as TicketStatus) ?? "InWork",
          priority: (data.priority as Priority) ?? "Normal",
          channel: data.channel ?? "web",
          tags: data.tags ?? [],
          createDate: data.createDate ?? new Date().toISOString(),
          lastUpdate: data.lastUpdate ?? data.createDate ?? new Date().toISOString(),
          requester: {
            id: data.requester?.id ?? undefined,
            name: data.requester?.name ?? data.requesterName ?? "Клиент",
            email: data.requester?.email ?? data.requesterContact ?? data.email ?? undefined,
            phone: data.requester?.phone ?? undefined,
          },
          assignedAgent:
            data.assignedAgent ??
            (data.workerId ? { id: data.workerId, name: data.workerId } : undefined),
          answer: data.answer ?? undefined,
          history: (data.history ?? data.messages ?? []).map((m: any, i: number) => ({
            id: m.id ?? i + 1,
            author: (m.author as "user" | "agent" | "system") ?? "user",
            text: m.text ?? "",
            createdAt: m.createdAt ?? new Date().toISOString(),
          })),
        };

        if (!cancelled) setTicket(detail);
      } catch (e: any) {
        if (!cancelled) setError(e?.message || "Ошибка загрузки");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [id]);

  const postJson = async (url: string, body: any) =>
    apiFetch(url, { method: "POST", body: JSON.stringify(body) });

  const closeWithReply = async () => {
    if (!ticket || !resolution.trim()) return;
    setSending(true);
    setBanner(null);
    try {
      await postJson(`/conversation/reply-close?id=${encodeURIComponent(ticket.conversationId)}`, {
        message: resolution,
      });

      // показываем успех и сразу возвращаем в список
      setBanner({ type: "success", text: "Ответ отправлен. Обращение закрыто." });

      // ждём чуть-чуть, чтобы пользователь успел увидеть сообщение
      setTimeout(() => navigate("/conversations", { replace: true }), 800);
    } catch (e: any) {
      setBanner({ type: "error", text: e?.message || "Ошибка при отправке ответа" });
    } finally {
      setSending(false);
    }
  };

  const saveDraft = async () => {
    if (!ticket || !resolution.trim()) return;
    setSending(true);
    setBanner(null);
    try {
      await postJson(`/conversation/draft?id=${encodeURIComponent(ticket.conversationId)}`, {
        message: resolution,
      });
      setBanner({ type: "success", text: "Черновик сохранён." });
    } catch (e: any) {
      setBanner({ type: "error", text: e?.message || "Ошибка при сохранении черновика" });
    } finally {
      setSending(false);
    }
  };

  const dot = (st?: TicketStatus) => (
    <span className="dot" style={{ background: statusDots[st || "New"] }} />
  );

  return (
    <div className="container py-4">
      <style>{styles}</style>

      <div className="header">
        <div>
          <div className="title">
            Обращение <span className="mono">{ticket?.number || "—"}</span>
          </div>
          <div className="muted">
            {dot(ticket?.status)}
            {ticket ? statusLabels[ticket.status] : "Загрузка…"}
            {ticket && <> • обновлено {new Date(ticket.lastUpdate).toLocaleString()}</>}
          </div>
        </div>
        <button className="btn secondary" onClick={() => navigate(-1)}>
          Назад
        </button>
      </div>

      {banner && <div className={`banner ${banner.type}`}>{banner.text}</div>}
      {error && <div className="banner error">{error}</div>}

      <div className="wrap">
        {/* Левая колонка */}
        <div className="card">
          <div className="section">
            <div className="label">Основная информация</div>
          </div>

          <div className="section">
            {loading && <div className="muted">Загрузка…</div>}
            {!loading && ticket && (
              <>
                <div className="grid" style={{ marginBottom: 10 }}>
                  <div>
                    <div className="label">Создано</div>
                    <div className="value">{new Date(ticket.createDate).toLocaleString()}</div>
                  </div>
                  <div>
                    <div className="label">Канал</div>
                    <div className="value">{ticket.channel}</div>
                  </div>
                  <div>
                    <div className="label">Заявитель</div>
                    <div className="value">{ticket.requester?.name || "—"}</div>
                  </div>
                  <div>
                    <div className="label">Контакты</div>
                    <div className="value">
                      {ticket.requester?.email || ticket.requester?.phone || "—"}
                    </div>
                  </div>
                  <div>
                    <div className="label">Приоритет</div>
                    <div className="value">{ticket.priority}</div>
                  </div>
                </div>

                {ticket.tags?.length > 0 && (
                  <div style={{ marginBottom: 12 }}>
                    <div className="label">Теги</div>
                    <div style={{ marginTop: 6 }}>
                      {ticket.tags.map((t) => (
                        <span key={t} className="pill tag">
                          {t}
                        </span>
                      ))}
                    </div>
                  </div>
                )}

                <div style={{ marginTop: 8 }}>
                  <div className="label">Тема</div>
                  <div className="value" style={{ marginTop: 4 }}>
                    {ticket.subject}
                  </div>
                </div>

                <div style={{ marginTop: 12 }}>
                  <div className="label">Описание</div>
                  <div className="value desc" style={{ marginTop: 4 }}>
                    {ticket.message || "—"}
                  </div>
                </div>

                {ticket.answer && (
                  <div style={{ marginTop: 16 }}>
                    <div className="label">Ответ агента</div>
                    <div className="value desc" style={{ marginTop: 4 }}>
                      {ticket.answer}
                    </div>
                  </div>
                )}
              </>
            )}
          </div>

          <div className="section">
            <div className="label" style={{ marginBottom: 8 }}>
              История
            </div>
            {loading && <div className="muted">Загрузка…</div>}
            {!loading && ticket && ticket.history?.length === 0 && (
              <div className="muted">История пуста</div>
            )}
            {!loading && ticket && ticket.history?.length > 0 && (
              <div className="timeline">
                {ticket.history.map((h) => (
                  <div className="tl-item" key={h.id}>
                    <div className="tl-dot" />
                    <div className="tl-head">
                      {h.author === "user"
                        ? "Клиент"
                        : h.author === "agent"
                        ? "Агент"
                        : "Система"}{" "}
                      • <span className="muted">{new Date(h.createdAt).toLocaleString()}</span>
                    </div>
                    <div className="tl-text">{h.text}</div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Правая колонка */}
        <div className="card">
          <div className="section">
            <div className="label">Ответ на обращение</div>
            <div className="muted" style={{ marginTop: 4 }}>
              Ваш ответ будет отправлен клиенту, после чего обращение автоматически закроется.
            </div>
          </div>

          <div className="section editor">
            <textarea
              placeholder="Кратко опишите решение..."
              value={resolution}
              onChange={(e) => setResolution(e.target.value)}
              disabled={sending || ticket?.status === "Closed"}
            />
            {ticket?.status === "Closed" && (
              <div className="muted" style={{ marginTop: 8 }}>
                Обращение уже закрыто.
              </div>
            )}
          </div>

          <div className="section">
            <div className="btns">
              <button
                className="btn"
                onClick={closeWithReply}
                disabled={sending || !resolution.trim() || ticket?.status === "Closed"}
              >
                {sending ? "Отправка…" : "Отправить и закрыть"}
              </button>
              <button
                className="btn secondary"
                onClick={saveDraft}
                disabled={sending || !resolution.trim() || ticket?.status === "Closed"}
              >
                Сохранить черновик
              </button>
              <button className="btn secondary" onClick={() => navigate(-1)}>
                Отмена
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

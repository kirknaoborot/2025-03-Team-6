import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";

type RoleLike = number | string | undefined | null;

interface ChannelDto {
  id: number;
  name: string;
  token: string;
  type: string;
}

interface CreatingChannelDto {
  name: string;
  token: string;
  type: string;
}

interface UpdatingChannelDto extends CreatingChannelDto {}

function isAdminRole(role: RoleLike): boolean {
  if (role === undefined || role === null) return false;
  const s = String(role).toLowerCase();
  return s === "0" || s === "administrator" || s === "администратор";
}

const styles = `
.wrap { max-width: 1100px; margin:0 auto; }
.header { display:flex; align-items:center; justify-content:space-between; margin-bottom:18px; }
.title { font-size:18px; font-weight:600; color:#111827; letter-spacing:.2px; }
.muted { color:#6b7280; font-size:13px; }
.card { border:1px solid #e5e7eb; border-radius:10px; background:#fff; }
.section { padding:16px 18px; }
.section + .section { border-top:1px solid #f3f4f6; }

.btn { border:1px solid #d1d5db; background:#111827; color:#fff; border-radius:8px; padding:10px 14px; font-weight:600; }
.btn.secondary { background:#fff; color:#111827; }
.btn.ghost { background:#fff; color:#111827; border-color:#e5e7eb; }
.btn.sm { padding:6px 10px; font-weight:600; border-radius:8px; }

.grid { display:grid; grid-template-columns: 1fr 1fr 1fr; gap:14px; }
@media (max-width: 900px){ .grid { grid-template-columns: 1fr; } }
.label { font-size:13px; color:#374151; margin-bottom:6px; }
.input, .select { width:100%; border:1px solid #e5e7eb; border-radius:8px; padding:10px 12px; font-size:14px; }

.table-wrap { overflow:auto; }
table { width:100%; border-collapse:collapse; }
thead th { position:sticky; top:0; background:#f9fafb; color:#374151; font-weight:600; font-size:13px; letter-spacing:.02em; border-bottom:1px solid #e5e7eb; padding:10px 12px; text-align:left; }
tbody td { padding:12px; border-bottom:1px solid #f3f4f6; font-size:14px; color:#111827; }
tbody tr:hover { background:#fcfcfd; }
.cell-actions { display:flex; gap:8px; }
.row-actions { display:flex; gap:8px; justify-content:flex-end; }

.error { color:#b91c1c; font-size:13px; }
.ok { color:#065f46; font-size:13px; }
.code { font-family: ui-monospace, SFMono-Regular, Menlo, monospace; }
`;

// API через шлюз
const BASE = import.meta.env.VITE_API_URL ?? "http://localhost:56466";
const LIST_PATH = "/channel/channels";       // GET список
const ITEM_PATH = "/channel/channel";        // POST/PUT/DELETE один

export default function ChannelSettings() {
  const navigate = useNavigate();

  const { accessToken, isAdmin } = useMemo(() => {
    try {
      const raw = localStorage.getItem("auth");
      const parsed = raw ? JSON.parse(raw) : null;
      return {
        accessToken: parsed?.accessToken as string | undefined,
        isAdmin: isAdminRole(parsed?.user?.role),
      };
    } catch {
      return { accessToken: undefined, isAdmin: false };
    }
  }, []);

  useEffect(() => {
    if (!isAdmin) navigate("/", { replace: true });
  }, [isAdmin, navigate]);

  const [items, setItems] = useState<ChannelDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [form, setForm] = useState<CreatingChannelDto>({
    name: "",
    token: "",
    type: "",
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [ok, setOk] = useState("");

  const load = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await fetch(`${BASE}${LIST_PATH}`, {
        headers: {
          Accept: "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.status === 401) {
        localStorage.removeItem("auth");
        navigate("/login", { replace: true });
        return;
      }
      if (!res.ok) throw new Error(await res.text());
      const data = (await res.json()) as ChannelDto[];
      setItems(data);
    } catch (e: any) {
      setError(e?.message || "Ошибка загрузки каналов");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!accessToken) {
      setLoading(false);
      setError("Нет accessToken");
      return;
    }
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [accessToken]);

  const setField =
    <K extends keyof CreatingChannelDto>(key: K) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
      setForm((f) => ({ ...f, [key]: e.target.value }));
    };

  const resetForm = () => {
    setForm({ name: "", token: "", type: "" });
    setEditingId(null);
    setError("");
    setOk("");
  };

  const startEdit = (c: ChannelDto) => {
    setEditingId(c.id);
    setForm({
      name: c.name ?? "",
      token: c.token ?? "",
      type: c.type ?? "",
    });
  };

  const validate = () => {
    if (!form.name.trim()) return "Укажите название";
    if (!form.type.trim()) return "Укажите тип";
    if (!form.token.trim()) return "Укажите токен";
    return "";
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setOk("");

    const msg = validate();
    if (msg) {
      setError(msg);
      return;
    }

    try {
      setSubmitting(true);

      const payload: CreatingChannelDto | UpdatingChannelDto = {
        name: form.name.trim(),
        token: form.token.trim(),
        type: form.type.trim(),
      };

      let url = `${BASE}${ITEM_PATH}`;
      let method: "POST" | "PUT" = "POST";

      if (editingId) {
        method = "PUT";
        // шлюз ожидает PUT /channel/channel?id={id}
        url = `${BASE}${ITEM_PATH}?id=${encodeURIComponent(editingId)}`;
      }

      const res = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify(payload),
      });

      if (res.status === 401) {
        localStorage.removeItem("auth");
        navigate("/login", { replace: true });
        return;
      }
      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || `HTTP ${res.status}`);
      }

      setOk(editingId ? "Изменения сохранены" : "Канал создан");
      await load();
      resetForm();
    } catch (e: any) {
      setError(e?.message || "Ошибка сохранения");
    } finally {
      setSubmitting(false);
    }
  };

  const remove = async (id: number) => {
    if (!confirm("Удалить канал?")) return;
    try {
      // шлюз ожидает DELETE /channel/channel?id={id}
      const res = await fetch(`${BASE}${ITEM_PATH}?id=${encodeURIComponent(id)}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${accessToken}` },
      });
      if (res.status === 401) {
        localStorage.removeItem("auth");
        navigate("/login", { replace: true });
        return;
      }
      if (!res.ok) throw new Error(await res.text());
      await load();
    } catch (e: any) {
      alert(e?.message || "Ошибка удаления");
    }
  };

  return (
    <div className="container py-4">
      <style>{styles}</style>

      <div className="wrap">
        <div className="header">
          <div>
            <div className="title">Настройки каналов</div>
            <div className="muted">Доступно только администраторам</div>
          </div>
          <div className="row-actions">
            <button className="btn secondary" type="button" onClick={() => navigate(-1)}>
              ← Назад
            </button>
          </div>
        </div>

        {/* Форма создания/редактирования */}
        <form className="card" onSubmit={onSubmit}>
          <div className="section">
            <div className="grid">
              <div>
                <div className="label">Название *</div>
                <input
                  className="input"
                  value={form.name}
                  onChange={setField("name")}
                  placeholder="Напр. Telegram / WhatsApp / Email"
                />
              </div>
              <div>
                <div className="label">Тип *</div>
                <input
                  className="input"
                  value={form.type}
                  onChange={setField("type")}
                  placeholder="Напр. telegram / whatsapp / email"
                />
              </div>
              <div>
                <div className="label">Токен *</div>
                <input
                  className="input code"
                  value={form.token}
                  onChange={setField("token")}
                  placeholder="Секретный токен/ключ интеграции"
                />
              </div>
            </div>

            <div style={{ marginTop: 12, display: "flex", gap: 8 }}>
              <button className="btn" type="submit" disabled={submitting}>
                {submitting ? "Сохранение…" : editingId ? "Сохранить" : "Создать"}
              </button>
              {editingId && (
                <button className="btn ghost" type="button" onClick={resetForm}>
                  Отмена
                </button>
              )}
            </div>

            <div style={{ marginTop: 10 }}>
              {error && <div className="error">{error}</div>}
              {ok && <div className="ok">{ok}</div>}
            </div>
          </div>
        </form>

        {/* Таблица каналов */}
        <div className="card" style={{ marginTop: 16 }}>
          <div className="section table-wrap">
            <table>
              <thead>
                <tr>
                  <th style={{ width: 80 }}>ID</th>
                  <th>Название</th>
                  <th style={{ width: 180 }}>Тип</th>
                  <th>Токен</th>
                  <th style={{ width: 220 }}></th>
                </tr>
              </thead>
              <tbody>
                {loading && (
                  <tr><td colSpan={5}>Загрузка…</td></tr>
                )}
                {error && !loading && (
                  <tr><td colSpan={5} style={{ color: "#b91c1c" }}>{error}</td></tr>
                )}
                {!loading && !error && items.map((c) => (
                  <tr key={c.id}>
                    <td>{c.id}</td>
                    <td>{c.name}</td>
                    <td>{c.type}</td>
                    <td className="code" title={c.token}>
                      {c.token?.length > 24 ? c.token.slice(0, 24) + "…" : c.token || "—"}
                    </td>
                    <td>
                      <div className="cell-actions">
                        <button className="btn sm ghost" type="button" onClick={() => startEdit(c)}>
                          Редактировать
                        </button>
                        <button className="btn sm ghost" type="button" onClick={() => remove(c.id)}>
                          Удалить
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
                {!loading && !error && items.length === 0 && (
                  <tr><td colSpan={5} style={{ color: "#6b7280", textAlign: "center", padding: "24px" }}>
                    Каналов нет
                  </td></tr>
                )}
              </tbody>
            </table>
          </div>
        </div>

      </div>
    </div>
  );
}

import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";

type RoleKey = "Administrator" | "Worker";

interface RegistrationRequestDto {
  fullName: string;
  login: string;    // логин (может быть email или просто уникальный логин)
  password: string;
  role: RoleKey | number; // на бэке enum: 0=Administrator, 1=Worker
}

type AuthStorage = {
  accessToken: string;
  user?: { role?: number | string };
};

const styles = `
.wrap { max-width: 780px; margin: 0 auto; }
.card { border:1px solid #e5e7eb; border-radius:12px; background:#fff; }
.section { padding:18px 20px; }
.header { display:flex; align-items:center; justify-content:space-between; margin-bottom:18px; }
.title { font-size:18px; font-weight:600; color:#111827; letter-spacing:.2px; }
.muted { color:#6b7280; font-size:13px; }
.grid { display:grid; grid-template-columns: 1fr 1fr; gap:16px; }
@media (max-width: 768px){ .grid { grid-template-columns: 1fr; } }
.label { font-size:13px; color:#374151; margin-bottom:6px; }
.input, .select { width:100%; border:1px solid #e5e7eb; border-radius:8px; padding:10px 12px; font-size:14px; }
.actions { display:flex; gap:10px; justify-content:flex-end; }
.btn { border:1px solid #d1d5db; background:#111827; color:#fff; border-radius:8px; padding:10px 14px; font-weight:600; }
.btn.secondary { background:#fff; color:#111827; }
.error { color:#b91c1c; font-size:13px; }
.ok { color:#065f46; font-size:13px; }
.tip { color:#6b7280; font-size:12px; margin-top:4px; }
`;

function isAdminRole(role: unknown): boolean {
  if (role === null || role === undefined) return false;
  const s = String(role).toLowerCase();
  return s === "0" || s === "administrator" || s === "администратор";
}

// необязательная проверка e-mail (если логин — email)
const emailRe =
  /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/i;

export default function RegisterUser() {
  const navigate = useNavigate();

  // читаем auth из localStorage
  const { accessToken, isAdmin } = useMemo(() => {
    try {
      const raw = localStorage.getItem("auth");
      const parsed = raw ? (JSON.parse(raw) as AuthStorage) : undefined;
      return {
        accessToken: parsed?.accessToken,
        isAdmin: isAdminRole(parsed?.user?.role),
      };
    } catch {
      return { accessToken: undefined, isAdmin: false };
    }
  }, []);

  // если не админ — возвращаем на список (доп. защита на фронте)
  if (!isAdmin) {
    navigate("/", { replace: true });
  }

  const [model, setModel] = useState<RegistrationRequestDto>({
    fullName: "",
    login: "",
    password: "",
    role: "Worker", // по умолчанию Работник
  });

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [ok, setOk] = useState("");

  const setField =
    <K extends keyof RegistrationRequestDto>(key: K) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
      setModel((m) => ({ ...m, [key]: e.target.value }));
    };

  // простая валидация
  const validate = () => {
    if (!model.fullName.trim()) return "Введите ФИО";
    if (!model.login.trim()) return "Введите логин";
    // если хочешь именно email — раскомментируй:
    // if (!emailRe.test(model.login.trim())) return "Укажите корректный e-mail";
    if (!model.password || model.password.length < 6)
      return "Пароль должен быть не короче 6 символов";
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
    if (!accessToken) {
      setError("Нет токена авторизации");
      return;
    }

    // enum маппинг: 0=Administrator, 1=Worker
    const roleValue =
      model.role === "Administrator" || model.role === 0 ? 0 : 1;

    const payload = {
      fullName: model.fullName.trim(),
      login: model.login.trim(),
      password: model.password,
      role: roleValue,
    };

    try {
      setSubmitting(true);
      const baseUrl = import.meta.env.VITE_API_URL ?? "http://localhost:56466";
      const res = await fetch(`${baseUrl}/user/create`, {
        method: "POST",
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
        const text = await res.text();
        throw new Error(text || `HTTP ${res.status}`);
      }

      setOk("Пользователь создан");
      // вернуть на список через секунду
      setTimeout(() => navigate("/", { replace: true }), 800);
    } catch (err: any) {
      setError(err?.message || "Ошибка создания пользователя");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container py-4">
      <style>{styles}</style>

      <div className="wrap">
        <div className="header">
          <div>
            <div className="title">Новый пользователь</div>
            <div className="muted">Доступно только администраторам</div>
          </div>
          <div className="actions">
            <button
              className="btn secondary"
              type="button"
              onClick={() => navigate(-1)}
            >
              ← Назад
            </button>
            <button
              className="btn"
              type="submit"
              form="userForm"
              disabled={submitting}
            >
              {submitting ? "Сохранение…" : "Сохранить"}
            </button>
          </div>
        </div>

        <form id="userForm" className="card" onSubmit={onSubmit}>
          <div className="section">
            <div className="grid">
              <div>
                <div className="label">ФИО *</div>
                <input
                  className="input"
                  value={model.fullName}
                  onChange={setField("fullName")}
                  placeholder="Иванов Иван Иванович"
                />
              </div>

              <div>
                <div className="label">Логин *</div>
                <input
                  className="input"
                  value={model.login}
                  onChange={setField("login")}
                  placeholder="email или уникальный логин"
                  autoComplete="username"
                />
                <div className="tip">
                  Можно указать e-mail (рекомендуется) или внутренний логин
                </div>
              </div>

              <div>
                <div className="label">Пароль *</div>
                <input
                  type="password"
                  className="input"
                  value={model.password}
                  onChange={setField("password")}
                  placeholder="минимум 6 символов"
                  autoComplete="new-password"
                />
              </div>

              <div>
                <div className="label">Роль</div>
                <select
                  className="select"
                  value={
                    typeof model.role === "number"
                      ? model.role === 0
                        ? "Administrator"
                        : "Worker"
                      : model.role
                  }
                  onChange={(e) =>
                    setModel((m) => ({
                      ...m,
                      role: e.target.value as RoleKey,
                    }))
                  }
                >
                  <option value="Worker">Работник</option>
                  <option value="Administrator">Администратор</option>
                </select>
              </div>
            </div>

            <div style={{ marginTop: 12 }}>
              {error && <div className="error">{error}</div>}
              {ok && <div className="ok">{ok}</div>}
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}

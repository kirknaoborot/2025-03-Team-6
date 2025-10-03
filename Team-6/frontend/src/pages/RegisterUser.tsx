import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createUser, RegistrationRequestDto } from "../services/userService";

export default function RegisterUser() {
  const navigate = useNavigate();

  const [form, setForm] = useState<RegistrationRequestDto>({
    fullName: "",
    login: "",
    password: "",
    role: 1, // Worker по умолчанию
  });

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      await createUser(form);
      navigate("/conversation/conversations"); // после успеха — к списку обращений
    } catch (err: any) {
      setError(err.message || "Ошибка при создании пользователя");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-5">
      <h2>Создание пользователя</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      <form onSubmit={handleSubmit} className="mt-3">
        <div className="mb-3">
          <label className="form-label">ФИО</label>
          <input
            type="text"
            name="fullName"
            className="form-control"
            value={form.fullName}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Логин</label>
          <input
            type="text"
            name="login"
            className="form-control"
            value={form.login}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Пароль</label>
          <input
            type="password"
            name="password"
            className="form-control"
            value={form.password}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Роль</label>
          <select
            name="role"
            className="form-select"
            value={form.role}
            onChange={(e) => setForm({ ...form, role: Number(e.target.value) })}
          >
            <option value={0}>Администратор</option>
            <option value={1}>Работник</option>
          </select>
        </div>

        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? "Создание..." : "Создать"}
        </button>
      </form>
    </div>
  );
}
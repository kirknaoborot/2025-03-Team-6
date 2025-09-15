import { useEffect, useState, useRef } from "react";
import { useParams } from "react-router-dom";

interface Message {
  id: number;
  author: "user" | "agent";
  text: string;
  createdAt: string;
}

export default function ConversationDetail() {
  const { id } = useParams(); // id обращения из URL
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMessage, setNewMessage] = useState("");
  const messagesEndRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    // тут можно загрузить переписку с бэкенда
    setMessages([
      { id: 1, author: "user", text: "Здравствуйте, у меня проблема с доступом", createdAt: "2025-09-15 13:45" },
      { id: 2, author: "agent", text: "Добрый день! Какой именно доступ не работает?", createdAt: "2025-09-15 13:46" },
    ]);
  }, [id]);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSend = () => {
    if (!newMessage.trim()) return;
    const msg: Message = {
      id: messages.length + 1,
      author: "agent",
      text: newMessage,
      createdAt: new Date().toLocaleTimeString(),
    };
    setMessages([...messages, msg]);
    setNewMessage("");
  };

  return (
    <div className="container mt-4 d-flex flex-column" style={{ height: "90vh" }}>
      {/* Заголовок */}
      <div className="mb-3">
        <h3>Обращение #{id}</h3>
        <p className="text-muted">Статус: В работе</p>
      </div>

      {/* Чат */}
      <div className="flex-grow-1 overflow-auto mb-3 border rounded p-3 bg-light">
        {messages.map((msg) => (
          <div
            key={msg.id}
            className={`d-flex mb-2 ${msg.author === "agent" ? "justify-content-end" : "justify-content-start"}`}
          >
            <div
              className={`p-2 rounded ${msg.author === "agent" ? "bg-primary text-white" : "bg-secondary text-white"}`}
              style={{ maxWidth: "70%" }}
            >
              <div>{msg.text}</div>
              <small className="d-block text-end opacity-75">{msg.createdAt}</small>
            </div>
          </div>
        ))}
        <div ref={messagesEndRef}></div>
      </div>

      {/* Поле ввода */}
      <div className="d-flex">
        <input
          type="text"
          className="form-control me-2"
          placeholder="Напишите ответ..."
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
        />
        <button className="btn btn-primary" onClick={handleSend}>
          Отправить
        </button>
      </div>
    </div> // <- Этот закрывающий тег был добавлен
  );
}
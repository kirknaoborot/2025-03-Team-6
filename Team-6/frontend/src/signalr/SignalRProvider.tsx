import React, { createContext, useContext, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";

/**
 * Тип контекста
 */
interface SignalRContextValue {
  connection: signalR.HubConnection | null;
  connected: boolean;
  connecting: boolean;
  start: () => Promise<void>;
  stop: () => Promise<void>;
}

/**
 * Создаём контекст SignalR
 */
const SignalRContext = createContext<SignalRContextValue>({
  connection: null,
  connected: false,
  connecting: false,
  start: async () => {},
  stop: async () => {},
});

/**
 * Хук для удобного доступа к контексту
 */
export const useSignalR = () => useContext(SignalRContext);

/**
 * Провайдер, создающий и управляющий глобальным соединением SignalR
 */
export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connected, setConnected] = useState(false);
  const [connecting, setConnecting] = useState(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  // Старт соединения
  const start = async () => {
    if (connecting || connected) return;
    if (!connectionRef.current) return;

    const conn = connectionRef.current;

    try {
      setConnecting(true);
      await conn.start();

      // Пример регистрации пользователя
      const raw = localStorage.getItem("auth");
      const auth = raw ? JSON.parse(raw) : null;
      const userId = auth?.user?.id ?? "anonymous";
      await conn.invoke("UserOnline", JSON.stringify({ id: userId }));

      console.log("✅ SignalR connected");
      setConnected(true);
    } catch (err) {
      console.error("❌ SignalR connection error:", err);
      setConnected(false);
    } finally {
      setConnecting(false);
    }
  };

  // Остановка соединения
  const stop = async () => {
    const conn = connectionRef.current;
    if (!conn) return;
    try {
      await conn.stop();
      setConnected(false);
      console.log("🔌 SignalR stopped");
    } catch {
      /* ignore */
    }
  };

  // Создание соединения (единожды при монтировании приложения)
  useEffect(() => {
    // если администратор — вообще не поднимаем SignalR
    try {
      const raw = localStorage.getItem("auth");
      const auth = raw ? JSON.parse(raw) : null;
      const role = String(auth?.user?.role ?? "").toLowerCase();
      const isAdmin = role === "0" || role === "administrator" || role === "администратор";
      if (isAdmin) {
        return; // не создаём соединение
      }
    } catch {}

    const conn = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:54000/onlinestatus", {
        withCredentials: true,
        // если у тебя авторизация по токену:
        // accessTokenFactory: () => localStorage.getItem("accessToken") || "",
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Подписка на события
    conn.on("UserCameOnline", (uid) => {
      console.log("🟢 UserCameOnline:", uid);
    });

    conn.on("ConversationDistributed", (msg) => {
      console.log("📨 ConversationDistributed:", msg);
      // Здесь можно, например, вызвать событие обновления списка разговоров
      window.dispatchEvent(new CustomEvent("conversation:update", { detail: msg }));
    });

    conn.onreconnected(() => {
      console.log("♻️ SignalR reconnected");
      setConnected(true);
    });

    conn.onclose(() => {
      console.log("❌ SignalR disconnected");
      setConnected(false);
    });

    connectionRef.current = conn;

    // Стартуем сразу при запуске
    start();

    // Чистим при закрытии вкладки
    return () => {
      conn.stop().catch(() => {});
      connectionRef.current = null;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <SignalRContext.Provider value={{ connection: connectionRef.current, connected, connecting, start, stop }}>
      {children}
    </SignalRContext.Provider>
  );
};

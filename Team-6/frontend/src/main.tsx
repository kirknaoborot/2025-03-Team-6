import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./styles/global.css";

// 👇 Импортируем провайдер
import { SignalRProvider } from "./signalr/SignalRProvider";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    {/* 👇 Оборачиваем всё приложение в провайдер SignalR */}
    <SignalRProvider>
      <App />
    </SignalRProvider>
  </React.StrictMode>
);
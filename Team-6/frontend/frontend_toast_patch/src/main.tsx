import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './index.css';
import Login from './pages/Login';
import Conversations from './pages/Conversations';
import ConversationDetail from './pages/ConversationDetail'; // Добавьте эту строку!
import 'bootstrap/dist/css/bootstrap.min.css';
import { ToastProvider } from './components/ToastProvider';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <ToastProvider>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/conversations" element={<Conversations />} />
        <Route path="/conversation/:id" element={<ConversationDetail />} />
      </Routes>
    </BrowserRouter>
  </ToastProvider>
  </React.StrictMode>
);
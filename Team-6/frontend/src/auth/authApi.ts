// Базы можно переопределить через .env (Vite)
// VITE_API_BASE — для защищённых запросов через Ocelot (по умолчанию http://localhost:5003)
// VITE_AUTH_BASE — для /auth/login и /auth/refresh (по умолчанию равно API_BASE)
const API_BASE = import.meta.env.VITE_API_BASE ?? 'http://localhost:56466';
const AUTH_BASE = import.meta.env.VITE_AUTH_BASE ?? API_BASE;

import { clearAuth, saveAuth, loadAuth } from './tokenStore';

export type AuthResponseDto = {
  accessToken: string;
  refreshToken: string;
  refreshTokenExpiryTime: string;
  user: {
    id: string;
    fullName: string;
    login: string;
    role: string;
    isActive: boolean;
  };
};

export async function login(login: string, password: string) {
  // Если хочешь бить мимо Ocelot прямо в сервис — установи VITE_AUTH_BASE=http://localhost:56468/api
  const res = await fetch(`${AUTH_BASE}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ login, password }),
  });
  if (!res.ok) throw new Error(await res.text());
  const data = (await res.json()) as AuthResponseDto;

  saveAuth({
    accessToken: data.accessToken,
    refreshToken: data.refreshToken,
    refreshTokenExpiryTime: data.refreshTokenExpiryTime,
    user: {
      id: data.user.id,
      fullName: data.user.fullName,
      login: data.user.login,
      role: String(data.user.role),
      isActive: data.user.isActive,
    },
  });

  return data;
}

export async function refresh() {
  const state = loadAuth();
  if (!state.refreshToken) throw new Error('No refresh token');

  const res = await fetch(`${AUTH_BASE}/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken: state.refreshToken }),
  });

  if (!res.ok) throw new Error(await res.text());
  const data = (await res.json()) as AuthResponseDto;

  saveAuth({
    accessToken: data.accessToken,
    refreshToken: data.refreshToken,
    refreshTokenExpiryTime: data.refreshTokenExpiryTime,
    user: {
      id: data.user.id,
      fullName: data.user.fullName,
      login: data.user.login,
      role: String(data.user.role),
      isActive: data.user.isActive,
    },
  });

  return data;
}

export function logout() { clearAuth(); }

export { API_BASE, AUTH_BASE };

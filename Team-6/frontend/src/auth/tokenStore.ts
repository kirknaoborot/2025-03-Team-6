export type AuthUser = {
  id: string;
  fullName: string;
  login: string;
  role: string;
  isActive: boolean;
};

export type AuthState = {
  accessToken: string | null;
  refreshToken: string | null;
  refreshTokenExpiryTime?: string | null;
  user: AuthUser | null;
};

const STORAGE_KEY = "auth";

let memory: AuthState = {
  accessToken: null,
  refreshToken: null,
  refreshTokenExpiryTime: null,
  user: null,
};

export function loadAuth() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (raw) memory = JSON.parse(raw);
  } catch {}
  return memory;
}

export function saveAuth(next: Partial<AuthState>) {
  memory = { ...memory, ...next };
  localStorage.setItem(STORAGE_KEY, JSON.stringify(memory));
}

export function clearAuth() {
  memory = { accessToken: null, refreshToken: null, refreshTokenExpiryTime: null, user: null };
  localStorage.removeItem(STORAGE_KEY);
}

export function getAccessToken() { return memory.accessToken; }
export function getRefreshToken() { return memory.refreshToken; }
export function getUser() { return memory.user; }

// Инициализация памяти при импорте модуля
loadAuth();

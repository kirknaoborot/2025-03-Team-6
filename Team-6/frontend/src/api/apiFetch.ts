import { getAccessToken, loadAuth } from '../auth/tokenStore';
import { refresh, logout, API_BASE } from '../auth/authApi';

let refreshPromise: Promise<any> | null = null;

export async function apiFetch<T = any>(
  input: string,
  init: RequestInit = {},
  { retryOn401 = true }: { retryOn401?: boolean } = {}
): Promise<T> {
  const url = input.startsWith('http') ? input : `${API_BASE}${input}`;
  const token = getAccessToken();

  const headers = new Headers(init.headers || {});
  if (!headers.has('Content-Type') && init.body && !(init.body instanceof FormData)) {
    headers.set('Content-Type', 'application/json');
  }
  if (token) headers.set('Authorization', `Bearer ${token}`);

  let res = await fetch(url, { ...init, headers });

  if (res.status === 401 && retryOn401) {
    try {
      if (!refreshPromise) {
        refreshPromise = refresh();
      }
      await refreshPromise;
    } catch {
      logout();
      throw new Error('Не авторизован (refresh failed)');
    } finally {
      refreshPromise = null;
    }

    const newToken = loadAuth().accessToken;
    const headers2 = new Headers(init.headers || {});
    if (!headers2.has('Content-Type') && init.body && !(init.body instanceof FormData)) {
      headers2.set('Content-Type', 'application/json');
    }
    if (newToken) headers2.set('Authorization', `Bearer ${newToken}`);

    res = await fetch(url, { ...init, headers: headers2 });
  }

  if (!res.ok) {
    const txt = await res.text().catch(() => '');
    throw new Error(txt || `Ошибка ${res.status}`);
  }

  const text = await res.text();
  return (text ? JSON.parse(text) : undefined) as T;
}

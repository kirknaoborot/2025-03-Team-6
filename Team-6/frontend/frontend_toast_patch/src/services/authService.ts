import { LoginRequestDto, AuthResponseDto } from '../types/authTypes';

const API_URL = 'http://localhost:56466/auth/login';

export async function loginUser(credentials: LoginRequestDto): Promise<AuthResponseDto> {
  const response = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    throw new Error('Неверный логин или пароль');
  }

  return await response.json();
}
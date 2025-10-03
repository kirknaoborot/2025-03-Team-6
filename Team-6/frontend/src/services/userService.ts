import { apiFetch } from "../api/apiFetch";

export type RegistrationRequestDto = {
  fullName: string;
  login: string;
  password: string;
  role: number; // 0=Administrator, 1=Worker
};

/** Создать пользователя (через API Gateway) */
export async function createUser(dto: RegistrationRequestDto): Promise<void> {
  await apiFetch<void>("/user/create", {
    method: "POST",
    body: JSON.stringify(dto),
  });
}
export interface LoginRequestDto {
  login: string;
  password: string;
}

export interface UserDto {
  id: number;
  login: string;
  fullName: string;
  email: string;
  roles: string[];
}

export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  refreshTokenExpiryTime: string;
  user: UserDto;
}
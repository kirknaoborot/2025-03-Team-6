using Auth.Core.Dto;

namespace Auth.Core.IServices;

public interface IAuthService
{
    Task<AuthResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<AuthResponseDto> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto);
}
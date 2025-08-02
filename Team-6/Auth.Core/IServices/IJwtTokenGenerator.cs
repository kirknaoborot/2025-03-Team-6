using Auth.Core.Dto;
using Auth.Domain.Entities;
using System.Security.Claims;

namespace Auth.Core.Services
{
    public interface IJwtTokenGenerator
    {
        Task<AuthResponseDto> GenerateAuthTokens(User user);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
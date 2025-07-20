using Auth.Core.Dto;
using Auth.Domain.Entities;
using System.Security.Claims;

namespace Auth.Core.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(ApplicationUser applicationUser, IEnumerable<string> roles);
        Task<AuthResponseDto> GenerateAuthTokens(ApplicationUser user, IEnumerable<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromToken(string token);
        DateTime GetRefreshTokenExpiryDate();
    }
}
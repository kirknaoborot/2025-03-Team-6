using CitizenRequest.Domain.Entities;

namespace Auth.Core.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
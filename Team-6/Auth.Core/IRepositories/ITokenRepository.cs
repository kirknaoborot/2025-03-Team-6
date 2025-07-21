using Auth.Domain.Entities;

namespace Auth.Core.IRepositories;

public interface ITokenRepository
{
    Task<RefreshToken> GetToken(Guid userId);
}
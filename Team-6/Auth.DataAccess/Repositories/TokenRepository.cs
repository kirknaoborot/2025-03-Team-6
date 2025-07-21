using Auth.Core.IRepositories;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.DataAccess.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly ApplicationDbContexts _dbContexts;

    public TokenRepository(ApplicationDbContexts dbContexts)
    {
        _dbContexts = dbContexts;
    }
    
    public async Task<RefreshToken> GetToken(Guid userId)
    {
        var token = await _dbContexts.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == userId);

        return token;
    }
}
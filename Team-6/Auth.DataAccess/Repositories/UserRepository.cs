using Auth.Core.IRepositories;
using Authorization.Models;
using Microsoft.EntityFrameworkCore;

namespace  Auth.DataAccess.Repositories
{
    public class UserRepository :IUserRepository
    {
        public readonly ApplicationDbContexts _dbContext;

        public UserRepository(ApplicationDbContexts dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> Get()
        {
            var authorEntities = await _dbContext.ApplicationUsers
              .AsNoTracking()
              .ToListAsync();
            var auth = authorEntities
                .Select(x => User.Create(x.FullName, x.Login, x.PasswordsHash, x.IsActive, x.Status, x.LastSeen, x.Role).user)
                .ToList();

            return auth;
        }
    }
}

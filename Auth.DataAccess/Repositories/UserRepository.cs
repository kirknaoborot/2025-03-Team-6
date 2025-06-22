using Auth.DataAccess;
using Authorization.Models;
using CitizenRequest.Core.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CitizenRequest.DataAccess.Repositories
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
                .Select(x => User.Create(x.Full_name, x.Login, x.Password_hash, x.Is_active, x.Status, x.Last_seen, x.Role).user)
                .ToList();

            return auth;
        }
    }
}

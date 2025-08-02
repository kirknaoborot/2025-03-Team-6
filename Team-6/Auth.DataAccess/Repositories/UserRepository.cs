using Auth.Core.IRepositories;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace  Auth.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContexts _dbContext;

        public UserRepository(ApplicationDbContexts dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> Get()
        {
            var users = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync();

            return users;
        }
        
        public async Task<User> Get(Guid id)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            return user;
        }
        
        public async Task<User> Get(string login)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Login.ToLower() == login.ToLower() && x.IsActive);

            return user;
        }

        public async Task Create(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            await _dbContext.Users
                .ExecuteUpdateAsync(x=> x.SetProperty(y => y.IsActive, false));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ProfileService.Application.Interfaces;
using ProfileService.Domain.Entities;
using ProfileService.Infrastructure.Context;

namespace ProfileService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProfileServiceContext _context;

        public UserRepository(ProfileServiceContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _context.Users
                .TagWith("Запрос получения списка пользователей")
                .Include(x => x.UserProfile)
                .Where(x => x.IsActive)
                .ToListAsync();

            return users;
        }

        public async Task<User> GetUser(Guid id)
        {
            var user = await _context.Users
                .TagWith($"Запрос получения пользователя по идентификатору: {id}")
                .Include(x => x.UserProfile)
                .SingleOrDefaultAsync(x => x.Id == id && x.IsActive);

            return user;
        }
    }
}

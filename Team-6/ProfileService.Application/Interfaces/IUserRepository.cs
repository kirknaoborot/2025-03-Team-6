using ProfileService.Domain.Entities;

namespace ProfileService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsers();

        Task<User> GetUser(Guid id);
    }
}

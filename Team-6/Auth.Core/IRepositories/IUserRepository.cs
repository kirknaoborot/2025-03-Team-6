using Auth.Domain.Entities;

namespace Auth.Core.IRepositories
{
    public interface IUserRepository
    {
        Task<List<User>> Get();

        Task<User> Get(Guid id);

        Task<User> Get(string login);

        Task Create(User user);

        Task Delete(Guid id);
    }
}
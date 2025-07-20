using Authorization.Models;

namespace Auth.Core.IRepositories
{
    public interface IUserRepository
    {
        Task<List<User>> Get();
    }
}
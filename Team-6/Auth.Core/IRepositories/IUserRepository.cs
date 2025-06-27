using Authorization.Models;

namespace CitizenRequest.Core.IRepositories
{
    public interface IUserRepository
    {
        Task<List<User>> Get();
    }
}
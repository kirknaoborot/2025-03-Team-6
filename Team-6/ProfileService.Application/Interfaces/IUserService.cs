using ProfileService.Application.DTO;

namespace ProfileService.Application.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// получение списка пользователей
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<UserDto>> GetAllUsers();

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserDto> GetUserById(Guid id);
    }
}

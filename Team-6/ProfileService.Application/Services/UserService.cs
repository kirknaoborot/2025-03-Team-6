using ProfileService.Application.DTO;
using ProfileService.Application.Interfaces;

namespace ProfileService.Application.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Метод получения списка пользователей
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetUsers();

            var result = users
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Login = x.Login,
                    IsActive = x.IsActive,
                    FirstName = x.UserProfile.FirstName,
                    LastName = x.UserProfile.LastName,
                    MiddleName = x.UserProfile.MiddleName
                })
                .ToList();

            return result;
        }

        /// <summary>
        /// Метод получения пользователя по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserById (Guid id)
        {
            var user = await _userRepository.GetUser(id);

            var result = user is null ? null : new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                IsActive = user.IsActive,
                FirstName = user.UserProfile.FirstName,
                LastName = user.UserProfile.LastName,
                MiddleName = user.UserProfile.MiddleName
            };

            return result;
        }
    }
}

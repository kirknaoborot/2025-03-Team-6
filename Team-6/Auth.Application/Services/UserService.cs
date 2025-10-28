using Auth.Application.Helpers;
using Auth.Core.Dto;
using Auth.Core.IRepositories;
using Auth.Core.IServices;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        

        public async Task<List<UserDto>> GetUsers()
        {
            var users = await _userRepository.Get();

            var result = users
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Login = x.Login,
                    Role = x.Role,
                    IsActive = x.IsActive,
                })
                .ToList();
            _logger.LogInformation($"{nameof(UserService)}.{nameof(GetUsers)}() -> Users found: {users.Count}");
            return result;
        }

        public async Task<UserDto> GetUser(Guid id)
        {
            var user = await _userRepository.Get(id);

            var result = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Login = user.Login,
                Role = user.Role,
                IsActive = user.IsActive,
            };
            _logger.LogInformation($"{nameof(UserService)}.{nameof(GetUser)}() -> User found: {user.FullName}");
            return result;
        }

        public async Task Delete(Guid id)
        {
            await _userRepository.Delete(id);
            _logger.LogInformation($"{nameof(UserService)}.{nameof(Delete)}() -> User with ID: {id} has been deleted");
        }

        public async Task Create(RegistrationRequestDto registrationRequestDto)
        {
            var user = new User
            {
                FullName = registrationRequestDto.FullName,
                Id = Guid.NewGuid(),
                PasswordsHash = PasswordHelper.HashPassword(registrationRequestDto.Password),
                IsActive = true,
                Login = registrationRequestDto.Login,
                Role = registrationRequestDto.Role
            };
            
            await _userRepository.Create(user);
            _logger.LogInformation($"{nameof(UserService)}.{nameof(Create)}() -> New user registered: {registrationRequestDto.FullName}");
        }
    }
}

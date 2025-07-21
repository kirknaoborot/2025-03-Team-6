using Auth.Application.Helpers;
using Auth.Core.Dto;
using Auth.Core.IServices;
using Auth.Domain.Entities;
using Auth.Core.IRepositories;

namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            return result;
        }

        public async Task Delete(Guid id)
        {
            await _userRepository.Delete(id);
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
        }
    }
}

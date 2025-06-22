using Auth.Core.Dto;
using Authorization.Models;

namespace Auth.Core.IServices
{
    public interface IUserService
    {
        Task<bool> AssignRole(string email, string roleName);
        Task<List<User>> GetUser();
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<string> Register(RegisterationRequestDto registrationRequestDto);
    }
}
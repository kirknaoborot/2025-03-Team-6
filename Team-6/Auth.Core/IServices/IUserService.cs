using Auth.Core.Dto;

namespace Auth.Core.IServices
{
    public interface IUserService
    {
        Task<List<UserDto>> GetUsers();
        Task<UserDto> GetUser(Guid id);
        Task Delete(Guid id);
        Task Create(RegistrationRequestDto registrationRequestDto);
    }
}
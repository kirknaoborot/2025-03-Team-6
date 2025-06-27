using ProfileService.Api.Models.Responses;
using ProfileService.Application.DTO;

namespace ProfileService.Api.Mapping
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(UserDto userDto)
        {
            return new UserResponse
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                MiddleName = userDto.MiddleName,
                Login = userDto.Login,
                IsActive = userDto.IsActive
            };
        }
    }
}

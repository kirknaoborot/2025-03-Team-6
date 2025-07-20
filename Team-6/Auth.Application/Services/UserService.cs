using Auth.Application.HubSignalR;
using Auth.Core.Dto;
using Auth.Core.IServices;
using Auth.DataAccess;
using Authorization.Models;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Auth.Core.Services;
using Auth.Core.IRepositories;


namespace Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContexts _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;

        public UserService(ApplicationDbContexts db, IJwtTokenGenerator jwtTokenGenerator,
           UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserRepository userRepository)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<AuthResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Login.ToLower() == loginRequestDto.Login.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password_hash);

            if (user == null || !isValid)
            {
                throw new UnauthorizedAccessException("Логин или пароль неверны");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var authTokens = await _jwtTokenGenerator.GenerateAuthTokens(user, roles);

            UserDto userDTO = new()
            {
                Login = user.Login,
                ID = user.Id,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Status = user.Status,
                LastSeen = user.LastSeen,
                Role = user.Role,
            };

            authTokens.User = userDTO;

            return authTokens;
        }

        //public async Task<string> Register(RegisterationRequestDto registrationRequestDto)
        //{
        //    var user = new ApplicationUser
        //    {
        //        UserName = registrationRequestDto.Login,
        //        Email = registrationRequestDto.Login,
        //        FullName = registrationRequestDto.FullName,
        //        IsActive = registrationRequestDto.IsActive,
        //        Status = registrationRequestDto.Status,
        //        LastSeen = registrationRequestDto.LastSeen,
        //        Role = registrationRequestDto.Role
        //    };

        //    var result = await _userManager.CreateAsync(user, registrationRequestDto.Password_hash);

        //    if (result.Succeeded)
        //    {
        //        if (!await _roleManager.RoleExistsAsync(registrationRequestDto.Role))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(registrationRequestDto.Role));
        //        }

        //        await _userManager.AddToRoleAsync(user, registrationRequestDto.Role);

        //        return "";
        //    }

        //    return result.Errors.FirstOrDefault()?.Description;
        //}

        public async Task<string> Register(RegisterationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.FullName,
                Email = registrationRequestDto.Login,
                FullName = registrationRequestDto.FullName,
                Login = registrationRequestDto.Login,
                IsActive = registrationRequestDto.IsActive,
                Status = registrationRequestDto.Status,
                LastSeen = registrationRequestDto.LastSeen,
                Role = registrationRequestDto.Role
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.PasswordHash);
                var errorDescriber = _userManager.ErrorDescriber as AuthErrorDescriber;

                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.FullName == registrationRequestDto.FullName);


                    if (!_roleManager.RoleExistsAsync(registrationRequestDto.Role).GetAwaiter().GetResult())
                    {
                        //create role if it does not exist
                        _roleManager.CreateAsync(new IdentityRole(registrationRequestDto.Role)).GetAwaiter().GetResult();
                    }


                    await _userManager.AddToRoleAsync(user, registrationRequestDto.Role);

                    UserDto userDto = new()
                    {
                        Login = userToReturn.Login,
                        ID = userToReturn.Id,
                        FullName = userToReturn.FullName,
                        IsActive = userToReturn.IsActive,
                        Status = userToReturn.Status,
                        LastSeen = userToReturn.LastSeen,
                        Role = userToReturn.Role
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }

            }
            catch (Exception ex)
            {

            }
            return "Error Encountered";
        }

        public async Task<List<User>> GetUser()
        {
            return await _userRepository.Get();
        }
    }
}

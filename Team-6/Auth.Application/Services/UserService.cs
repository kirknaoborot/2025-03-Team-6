using Auth.Application.HubSignalR;
using Auth.Core.Dto;
using Auth.Core.IServices;
using Auth.DataAccess;
using Authorization.Models;
using CitizenRequest.Core.IRepositories;
using CitizenRequest.Domain.Entities;
using Microsoft.AspNetCore.Identity;

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

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Login.ToLower() == loginRequestDto.Login.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password_hash);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDTO = new()
            {
                Login = user.Login,
                ID = user.Id,
                Full_name = user.Full_name,
                Is_active = user.Is_active,
                Status = user.Status,
                Last_seen = user.Last_seen,
                Role = user.Role,
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegisterationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Full_name,
                Email = registrationRequestDto.Login,
                Full_name = registrationRequestDto.Full_name,
                Login = registrationRequestDto.Login,
                Is_active = registrationRequestDto.Is_active,
                Status = registrationRequestDto.Status,
                Last_seen = registrationRequestDto.Last_seen,
                Role = registrationRequestDto.Role
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password_hash);
                var errorDescriber = _userManager.ErrorDescriber as AuthErrorDescriber;

                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.Full_name == registrationRequestDto.Full_name);


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
                        Full_name = userToReturn.Full_name,
                        Is_active = userToReturn.Is_active,
                        Status = userToReturn.Status,
                        Last_seen = userToReturn.Last_seen,
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

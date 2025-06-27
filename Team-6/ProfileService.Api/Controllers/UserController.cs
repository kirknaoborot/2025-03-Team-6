using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Api.Mapping;
using ProfileService.Api.Models.Responses;
using ProfileService.Application.Interfaces;

namespace ProfileService.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        /// <summary>
        /// Метод получения списка активных пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> Get()
        {
            var users = await _userService.GetAllUsers();

            var result = users
                .Select(x => UserMapper.ToResponse(x))
                .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Метод получения пользователя по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserResponse>> Get(Guid id)
        {
            var user = await _userService.GetUserById(id);

            if (user is null)
            {
                return NotFound();
            }

            var result = UserMapper.ToResponse(user);

            return Ok(result);
        }
    }
}

using Auth.Core.Dto;
using Auth.Core.IServices;
using AuthService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// <summary>
    /// Метод создания нового пользователя
    /// </summary>
    /// <returns></returns>
    [HttpPost("user")]
    public async Task<IActionResult> Create([FromBody] RegistrationRequestDto model)
    {
        try
        {
            await _userService.Create(model);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
    
    /// <summary>
    /// Метод получения списка пользователей
    /// </summary>
    /// <returns></returns>
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> Get()
    {
        var users = await _userService.GetUsers();

        return Ok(users);
    }

    /// <summary>
    /// Метод получения пользователя по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("user/{id}")]
    public async Task<ActionResult<AuthResponse>> Get(Guid id)
    {
        var user = await _userService.GetUser(id);

        return Ok(user);
    }

    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("user")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.Delete(id);

        return Ok();
    }
    /*
    /// <summary>
    /// Метод получения активных пользователей
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-active-users")]
    public async Task<ActionResult<List<AuthResponse>>> GetActiveUsers()
    {
        try
        {
            var activeUsersList = new List<AuthResponse>();

            foreach (var kvp in _activeUsers)
            {
                var user = kvp.Value;
                var roles = await _userManager.GetRolesAsync(user);

                activeUsersList.Add(new AuthResponse(
                    user.FullName ?? "",
                    "true",
                    user.Status ?? "online",
                    user.LastSeen ?? DateTime.UtcNow,
                    string.Join(",", roles)
                ));
            }

            return Ok(activeUsersList);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Ошибка при получении активных пользователей: {ex.Message}");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }*/
}
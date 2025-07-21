using Auth.Core.Dto;
using Auth.Core.IServices;
using AuthService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        userService = _userService;
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
}
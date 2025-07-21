using Auth.Core.Dto;
using Auth.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        /// <summary>
        /// Метод входа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                var authResponse = await _authService.Login(model);
                
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Метод обновления токена
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var result = await _authService.RefreshToken(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized();
            }
        }
    }
}

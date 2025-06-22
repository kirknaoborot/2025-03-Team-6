using Auth.Core.Dto;
using Auth.Core.IServices;
using AuthService.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [Authorize]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IUserService _authService;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;


        public AuthAPIController(IUserService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
            _response = new();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDto model)
        {

            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Логин или пароль неверны";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpGet]
        public async Task<ActionResult<List<AuthResponse>>> Get()
        {
            var author = await _authService.GetUser();
            var response = from c1 in author
                           select new
                           {
                               Full_name = c1.Full_name,
                               Is_active = c1.Is_active,
                               Status = c1.Status,
                               Last_seen = c1.Last_seen,
                               Role = c1.Role,
                           };
            return Ok(response);
        }
    }
}

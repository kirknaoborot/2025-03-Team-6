using Auth.Core.Dto;
using Auth.Core.IServices;
using Auth.Core.Services;
using Auth.DataAccess;
using Auth.Domain.Entities;
using AuthService.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IUserService _authService;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContexts _context;
        private static readonly ConcurrentDictionary<string, ApplicationUser> _activeUsers = new();


        public AuthAPIController(IUserService authService, IConfiguration configuration, IJwtTokenGenerator tokenGenerator, UserManager<ApplicationUser> userManager, ApplicationDbContexts context)
        {
            _authService = authService;
            _configuration = configuration;
            _response = new();
            _tokenGenerator = tokenGenerator;
            _userManager = userManager; 
            _context = context;
        }

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                var authResponse = await _authService.Login(model);
                _response.Result = authResponse;

        
                if (authResponse.User != null)
                {
                    var user = await _userManager.FindByIdAsync(authResponse.User.ID);
                    if (user != null)
                    {
                        user.LastSeen = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);

            
                        _activeUsers.AddOrUpdate(authResponse.User.ID, user, (key, oldValue) => user);
                        System.Console.WriteLine($"Пользователь {user.FullName} ({user.UserName}) вошел в систему и добавлен в список активных.");
                    }
                }
                return Ok(_response);
            }
            catch (UnauthorizedAccessException)
            {
                _response.IsSuccess = false;
                _response.Message = "Логин или пароль неверны";
                return BadRequest(_response);
            }
        }

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
        }


        [HttpGet]
        public async Task<ActionResult<List<AuthResponse>>> Get()
        {
            var author = await _authService.GetUser();
            var response = from c1 in author
                           select new
                           {
                               FullName = c1.FullName,
                               IsActive = c1.IsActive,
                               Status = c1.Status,
                               LastSeen = c1.LastSeen,
                               Role = c1.Role,
                           };
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _response.IsSuccess = false;
                _response.Message = "Invalid refresh token";
                return BadRequest(_response);
            }

            // 1. Извлекаем access token из заголовка
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(accessToken))
            {
                _response.IsSuccess = false;
                _response.Message = "Отсутствует токен доступа";
                return BadRequest(_response);
            }

            // 2. Валидируем access token и получаем claims
            var principal = _tokenGenerator.GetPrincipalFromToken(accessToken);
            if (principal == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Неверный токен доступа";
                return BadRequest(_response);
            }

            var userId = principal.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _response.IsSuccess = false;
                _response.Message = "Неверный ID пользователя в токене";
                return BadRequest(_response);
            }

            // 3. Находим refresh token в БД
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed)
            {
                _response.IsSuccess = false;
                _response.Message = "Недействительный или отозванный токен Refresh";
                return BadRequest(_response);
            }

            if (storedToken.Expires < DateTime.UtcNow)
            {
                _response.IsSuccess = false;
                _response.Message = "Refresh token истёк";
                return BadRequest(_response);
            }

            // 4. Проверяем, что UserId из access token совпадает с тем, кто владеет refresh token
            if (storedToken.UserId != userId)
            {
                _response.IsSuccess = false;
                _response.Message = "Refresh token не соответствует пользователю";
                return BadRequest(_response);
            }

            // 5. Получаем пользователя
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Пользователь не найден";
                return BadRequest(_response);
            }

            var roles = await _userManager.GetRolesAsync(user);

            // 6. Генерируем новые токены
            var newAuthTokens = await _tokenGenerator.GenerateAuthTokens(user, roles);

            // 7. Отмечаем старый refresh token как использованный
            storedToken.IsUsed = true;
            await _context.SaveChangesAsync();

            _response.Result = newAuthTokens;
            return Ok(_response);
        }
    }
}

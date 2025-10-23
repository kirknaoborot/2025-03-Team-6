using Auth.Application.Helpers;
using Auth.Core.Dto;
using Auth.Core.IRepositories;
using Auth.Core.IServices;
using Auth.Core.Services;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, ITokenRepository tokenRepository,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var user = await _userRepository.Get(loginRequestDto.Login);

        if (user is null)
        {
            _logger.LogError($"Пользователь {loginRequestDto.Login} не найден");
            throw new UnauthorizedAccessException("User not found");
        }

        var checkPassword = PasswordHelper.Verify(loginRequestDto.Password, user.PasswordsHash);

        if (!checkPassword)
        {
            _logger.LogError("Неверный логин или пароль");
            throw new UnauthorizedAccessException("Invalid password");
        }

        var tokens = await _jwtTokenGenerator.GenerateAuthTokens(user);

        var result = new AuthResponseDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiryTime = tokens.RefreshTokenExpiryTime,
            User = new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            }
        };
        _logger.LogInformation($"В систему вошел пользователь: {user.FullName}");
        return result;
    }

    public async Task<AuthResponseDto> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var principals = _jwtTokenGenerator.GetPrincipalFromToken(refreshTokenRequestDto.AccessToken);

        if (principals is null)
        {
            _logger.LogError("Ошибка при получении токена");
            throw new UnauthorizedAccessException();
        }
        
        var userId = principals.FindFirstValue("id") ?? throw new UnauthorizedAccessException();

        var tokenModel = await _tokenRepository.GetToken(Guid.Parse(userId));

        if (tokenModel is null)
        {
            _logger.LogError("Ошибка при получении токена");
            throw new UnauthorizedAccessException();
        }

        if (tokenModel.IsUsed == false || !string.Equals(tokenModel.Token, refreshTokenRequestDto.RefreshToken) ||
            tokenModel.Expires < DateTime.UtcNow)
        {
            _logger.LogError("Ошибка при получении токена");
            throw new UnauthorizedAccessException();
        }
        
        var user = await _userRepository.Get(Guid.Parse(userId));
        var tokens = await _jwtTokenGenerator.GenerateAuthTokens(user);
        
        var result = new AuthResponseDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiryTime = tokens.RefreshTokenExpiryTime,
            User = new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            }
        };
        _logger.LogInformation("Получен новый токен");
        return result;
    }
}

using System.Text.Json;
using OperatorStatusService.Domain.Models;
using OperatorStatusService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace OperatorStatusService.Infrastructure.Services
{
    public class AuthApiService : IOperatorStatusService
    {
        private readonly HttpClient _httpClient;
        private readonly string _authApiUrl;

        public AuthApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authApiUrl = configuration["AppSettings:AuthApi:Url"] ?? "https://localhost:61191/api/auth/get-active-users";
        }

        public async Task<List<UserInfo>> GetActiveOperatorsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(_authApiUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка HTTP: {response.StatusCode}");
                    return new List<UserInfo>();
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                var users = JsonSerializer.Deserialize<List<UserInfo>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (users == null)
                    return new List<UserInfo>();

                return users
                    .Where(u =>
                        u != null &&
                        u.Role?.Contains("operator", StringComparison.OrdinalIgnoreCase) == true &&
                        !string.IsNullOrEmpty(u.IsActiveRaw))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге: {ex.Message}");
                return new List<UserInfo>();
            }
        }
    }
}
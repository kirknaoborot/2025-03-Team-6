using Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Shared.Extensions
{
    /// <summary>
    /// Расширения для работы с зашифрованными строками подключения
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Получить расшифрованную строку подключения из конфигурации
        /// </summary>
        /// <param name="configuration">Конфигурация</param>
        /// <param name="connectionStringName">Имя строки подключения</param>
        /// <returns>Расшифрованная строка подключения</returns>
        public static string GetDecryptedConnectionString(this IConfiguration configuration, string connectionStringName)
        {
            var encryptedConnectionString = configuration.GetConnectionString(connectionStringName);
            return ConnectionStringEncryption.Decrypt(encryptedConnectionString);
        }
    }
}

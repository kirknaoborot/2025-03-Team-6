using System.Text;

namespace Infrastructure.Shared.Services
{
    /// <summary>
    /// Сервис для шифрования/дешифрования строк подключения в Base64
    /// </summary>
    public static class ConnectionStringEncryption
    {
        /// <summary>
        /// Зашифровать строку подключения в Base64
        /// </summary>
        /// <param name="connectionString">Исходная строка подключения</param>
        /// <returns>Зашифрованная строка в Base64</returns>
        public static string Encrypt(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(connectionString);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Расшифровать строку подключения из Base64
        /// </summary>
        /// <param name="encryptedConnectionString">Зашифрованная строка в Base64</param>
        /// <returns>Расшифрованная строка подключения</returns>
        public static string Decrypt(string encryptedConnectionString)
        {
            if (string.IsNullOrEmpty(encryptedConnectionString))
                return string.Empty;

            try
            {
                var bytes = Convert.FromBase64String(encryptedConnectionString);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                // Если строка не является валидным Base64, возвращаем как есть
                // Это позволяет использовать обычные строки подключения без шифрования
                return encryptedConnectionString;
            }
        }
    }
}

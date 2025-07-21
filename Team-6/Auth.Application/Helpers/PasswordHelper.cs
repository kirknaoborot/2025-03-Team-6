namespace Auth.Application.Helpers;

public static class PasswordHelper
{
    /// <summary>
    /// Метод получения хэш пароля пользователя
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Метод опредления валидности пароля
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    public static bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
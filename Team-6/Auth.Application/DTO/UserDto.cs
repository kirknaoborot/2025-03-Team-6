using Infrastructure.Shared.Enums;

namespace Auth.Application.DTO;

public class UserDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }
        
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FullName { get; set; }
        
    /// <summary>
    /// Логин пользователя
    /// </summary>
    public string Login { get; set; }
        
    /// <summary>
    /// ХЭШ - пароль
    /// </summary>
    public string PasswordsHash { get; set; }
        
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public RoleType Role { get; set; }
        
    /// <summary>
    /// Флаг удален или не удален
    /// </summary>
    public bool IsActive { get; set; }
}
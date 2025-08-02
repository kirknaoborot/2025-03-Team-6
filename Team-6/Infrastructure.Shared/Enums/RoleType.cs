using Microsoft.OpenApi.Attributes;

namespace Infrastructure.Shared.Enums;

public enum RoleType
{
    [Display("Администратор")]
    Administrator = 0,
    
    [Display("Работник")]
    Worker = 1,
}
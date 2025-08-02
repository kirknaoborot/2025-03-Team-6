

using System.ComponentModel.DataAnnotations;
using Infrastructure.Shared.Enums;

namespace Auth.Core.Dto
{
    public class RegistrationRequestDto
    {
        /// <summary>
        /// ФИО пользователя
        /// </summary>
        [Required]
        public string FullName { get; set; }
        
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required]
        public string Login { get; set; }
        
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required]
        public string Password { get; set; }
        
        /// <summary>
        /// Роль
        /// </summary>
        public RoleType Role { get; set; }
    }
}

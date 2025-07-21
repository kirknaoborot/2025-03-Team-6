using System.ComponentModel.DataAnnotations;

namespace Auth.Domain.Entities
{
    public class RefreshToken
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Токен доступа
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// Период доступа
        /// </summary>
        public DateTime Expires { get; set; }
        
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Флаг отозван ли токен
        /// </summary>
        public bool IsUsed { get; set; }
    }
}

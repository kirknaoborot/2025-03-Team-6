using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Domain.Entities
{
    [Table("refresh_tokens", Schema = "auth")]
    public class RefreshToken
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Токен доступа
        /// </summary>
        [Column("token")]
        public string Token { get; set; }

        /// <summary>
        /// Период доступа
        /// </summary>
        [Column("expires")]
        public DateTime Expires { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Column("user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Флаг отозван ли токен
        /// </summary>
        [Column("is_used")]
        public bool IsUsed { get; set; }
    }
}

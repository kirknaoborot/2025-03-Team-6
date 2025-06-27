namespace ProfileService.Domain.Entities
{
    public class User
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Флаг активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата изменения записи
        /// </summary>
        public DateTime EditDate { get; set; } = DateTime.Now;

        public virtual UserProfile UserProfile { get; set; }
    }
}

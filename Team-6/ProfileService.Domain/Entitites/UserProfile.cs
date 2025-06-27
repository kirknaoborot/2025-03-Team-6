namespace ProfileService.Domain.Entities
{
    public class UserProfile
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }


        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime EditDate { get; set; } = DateTime.Now;

        public virtual User User { get; set; }
    }
}

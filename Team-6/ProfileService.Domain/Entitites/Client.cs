namespace ProfileService.Domain.Entities
{
    public class Client
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата обновления
        /// </summary>
        public DateTime EditDate { get; set; } = DateTime.Now;
    }
}

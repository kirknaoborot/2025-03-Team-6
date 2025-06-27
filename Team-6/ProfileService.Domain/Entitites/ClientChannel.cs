namespace ProfileService.Domain.Entities
{
    public class ClientChannel
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Тип внешнего канала
        /// </summary>
        public string ExternalChannelType { get; set; }

        /// <summary>
        /// Идентификатор во внешних каналах
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime EditDate { get; set; } = DateTime.Now;
    }
}

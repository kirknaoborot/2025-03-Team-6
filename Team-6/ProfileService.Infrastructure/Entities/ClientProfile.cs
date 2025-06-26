
namespace ProfileService.Infrastructure.Entities
{
    internal class ClientProfile
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Идентификатор клиента в ТГ
        /// </summary>
        public string TelegramId { get; set; }

        /// <summary>
        /// Идентификатор в ВК
        /// </summary>
        public string VkId { get; set; }
    }
}

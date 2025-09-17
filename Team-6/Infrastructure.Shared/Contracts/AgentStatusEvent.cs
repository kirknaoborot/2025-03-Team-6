using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts
{
    public class AgentStatusEvent
    {
        /// <summary>
        /// Идентификатор оператора
        /// </summary>
        public Guid AgentId { get; set; }
        
        /// <summary>
        /// Статус агента
        /// </summary>
        public AgentStatusType Status { get; set; }

        /// <summary>
        /// Время события
        /// </summary>
        public DateTime Date { get; set; }
    }
}

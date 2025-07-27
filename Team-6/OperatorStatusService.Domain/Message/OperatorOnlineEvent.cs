
namespace OperatorStatusService.Domain.Message
{
    public class OperatorOnlineEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}

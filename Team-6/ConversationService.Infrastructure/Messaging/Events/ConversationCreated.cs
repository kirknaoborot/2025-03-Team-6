namespace ConversationService.Infrastructure.Messaging.Events;

public class ConversationCreated
{
    public Guid ConversationId { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public Guid WorkerId { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}
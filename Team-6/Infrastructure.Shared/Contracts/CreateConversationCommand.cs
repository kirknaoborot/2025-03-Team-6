namespace Infrastructure.Shared.Contracts;

public class CreateConversationCommand
{
    public Guid ConversationId { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public string Channel { get; set; }
    public Guid WorkerId { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}
using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts;

public class ConversationBase
{
	public Guid ConversationId { get; set; }
	public string Message { get; set; }
	public StatusType Status { get; set; }
	public ChannelType Channel { get; set; }
	public Guid WorkerId { get; set; }
	public DateTimeOffset CreateDate { get; set; }
}

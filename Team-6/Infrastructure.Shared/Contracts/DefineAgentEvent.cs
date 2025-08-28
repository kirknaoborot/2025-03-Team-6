using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts;

public class DefineAgentEvent
{
	public Guid ConversationId { get; set; }
	public Guid WorkerId { get; set; }
	public string MessageText { get; set; }
	public DateTimeOffset CreateDate { get; set; }
	public ChannelType Channel { get; set; }
}

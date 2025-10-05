using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts;

public class ConversationBase
{
	public Guid ConversationId { get; set; }
	public long UserId { get; set; }
	public string Message { get; set; }
	public StatusType Status { get; set; }
	public ChannelType Channel { get; set; }
	public Guid WorkerId { get; set; }
	public DateTimeOffset CreateDate { get; set; }
	public string BotToken { get; set; }
	public string Answer { get; set; }
	public int ChannelSettingsId { get; set; }
}

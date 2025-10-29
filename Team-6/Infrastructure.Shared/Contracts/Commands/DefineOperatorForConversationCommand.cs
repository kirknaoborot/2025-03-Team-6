using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts;

public class DefineOperatorForConversationCommand
{
	public Guid ConversationId { get; set; }
	public long UserId { get; set; }
	public string MessageText { get; set; }
	public DateTimeOffset CreateDate { get; set; }
	public ChannelType Channel { get; set; }
	public int ChannelSettingsId { get; set; }
}

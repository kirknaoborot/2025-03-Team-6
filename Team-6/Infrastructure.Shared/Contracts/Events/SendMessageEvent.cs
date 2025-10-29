using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts;

public class SendMessageEvent
{
	public long UserId { get; set; }
	public string MessageText { get; set; }
	public ChannelType Channel { get; set; }
	public int ChannelSettingsId { get; set; }
}

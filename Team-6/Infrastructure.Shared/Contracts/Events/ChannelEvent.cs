using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts;

public class ChannelEvent
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Token { get; set; }
	public ChannelType Type { get; set; }
	public ChannelInfoAction Action { get; set; }
}

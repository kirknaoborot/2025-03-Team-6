using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;

namespace MessageHubService.Application.Interfaces
{
	public interface IBot
	{
		Task CreateBotAsync(ChannelEvent channelEvent);
		Task StartAsync();
		Task StopAsync();
		Task SentMessageToClientAsync(SendMessageDto sendMessageDto);
		int GetHashCode();
		void Dispose();
	}
}

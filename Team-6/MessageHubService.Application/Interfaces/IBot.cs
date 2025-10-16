using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;

namespace MessageHubService.Application.Interfaces
{
	public interface IBot
	{
		void CreateBot(ChannelEvent channelEvent);
		Task StartAsync();
		Task StopAsync();
		Task SentMessageToClientAsync(SendMessageDto sendMessageDto);
		int GetHashCode();
	}
}

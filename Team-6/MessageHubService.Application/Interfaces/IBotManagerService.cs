using Infrastructure.Shared.Contracts;

namespace MessageHubService.Application.Interfaces;

public interface IBotManagerService
{
	Task CreateBotAsync(ChannelEvent channelEvent);
	Task UpdateBotAsync(ChannelEvent channelEvent);
	Task DeleteBotAsync(ChannelEvent channelEvent);
	bool TryGetBot(int id, out IBot? telegramBot);
}

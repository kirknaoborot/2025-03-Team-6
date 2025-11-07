using Infrastructure.Shared.Contracts;

namespace MessageHubService.Application.Interfaces;

public interface IBotManagerService
{
	Task CreateBotAsync(ChannelCommand channelEvent);
	Task UpdateBotAsync(ChannelCommand channelEvent);
	Task DeleteBotAsync(ChannelCommand channelEvent);
	bool TryGetBot(int id, out IBot? telegramBot);
}

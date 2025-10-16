using Infrastructure.Shared.Contracts;
using MessageHubService.Application.Services.TelegramBot;

namespace MessageHubService.Application.Interfaces
{
	public interface IBotService
	{
		Task AddBot(ChannelEvent channelEvent);
		bool TryGetTelegramBot(int id, out TelegramBotService telegramBot);
	}
}

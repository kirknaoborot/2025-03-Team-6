using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Application.Interfaces;
using MessageHubService.Application.Services.TelegramBot;
using MessageHubService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Application.Services
{
    public class SendMessageService : IBotService
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, TelegramBotService> _telegramBots = new();
        private readonly ILogger<SendMessageService> _logger;

        public SendMessageService(ILogger<SendMessageService> logger, IConfiguration configuration)
        {
            _logger = logger;
        }

		public async Task AddBot(ChannelEvent channelEvent)
		{
			_logger.LogInformation($"{nameof(SendMessageService)}.{nameof(AddBot)}() -> channel event is null {channelEvent is null}");

			await StartBotAsync(channelEvent);
		}

        public bool TryGetTelegramBot(int id, out TelegramBotService telegramBot)
		{
            var ok = _telegramBots.TryGetValue(id, out var bot);
            telegramBot = bot;
            return ok;
		}

        private async Task StartBotAsync(ChannelEvent channelEvent)
        {
            try
            {
                var telegramBot = new TelegramBotService();
                telegramBot.ChannelId = channelEvent.Id;
                telegramBot.SetTokenAndName(channelEvent.Token, channelEvent.Name);
                telegramBot.DefineTelegramBotClient();

                _logger.LogInformation("Starting bot {BotName} (ChannelId {ChannelId})", telegramBot.Name, telegramBot.ChannelId);

                _telegramBots[telegramBot.ChannelId] = telegramBot;
                await telegramBot.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start bot {ChannelId}", channelEvent.Id);
            }
        }
    }
}

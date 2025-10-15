using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Application.Services.TelegramBot;
using MessageHubService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Application.Services
{
    public class SendMessageService : BackgroundService
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, TelegramBotService> _telegramBots = new();
        private readonly ILogger<SendMessageService> _logger;
        private readonly IBus _bus;

        public SendMessageService(ILogger<SendMessageService> logger, IConfiguration configuration, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(SendMessageService)}.{nameof(ExecuteAsync)}() -> Background service is starting.");
        }

		public async Task AddBot(ChannelEvent channelEvent)
		{
			_logger.LogInformation($"{nameof(SendMessageService)}.{nameof(AddBot)}() -> channel event is null {channelEvent is null}");

			await StartBotAsync(channelEvent);
		}

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(SendMessageService)}.{nameof(StopAsync)}() -> Background service is stopping.");

            var stopTasks = _telegramBots.Values.Select(b => b.StopAsync());
            await Task.WhenAll(stopTasks);

            _telegramBots.Clear();
        }

        public static bool TryGetTelegramBot(int id, out TelegramBotService telegramBot)
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

                telegramBot.OnIncomingMessage += (sender, args) =>
                {
                    _ = HandleIncomingMessageAsync(args);
                };

                _logger.LogInformation("Starting bot {BotName} (ChannelId {ChannelId})", telegramBot.Name, telegramBot.ChannelId);

                _telegramBots[telegramBot.ChannelId] = telegramBot;
                await telegramBot.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start bot {ChannelId}", channelEvent.Id);
            }
        }

        private async Task HandleIncomingMessageAsync(MessageEventArgs e)
        {
            try
            {
                _logger.LogInformation("Incoming message {MessageId} from {UserId} via channel {ChannelId}", e.Id, e.UserId, e.ChannelSettingId);

                var newInMessage = new ClientMessageEvent
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    MessageText = e.Text,
                    SendData = e.SendData,
                    Priority = "high",
                    Channel = Infrastructure.Shared.Enums.ChannelType.Telegram,
                    ChannelSettingsId = e.ChannelSettingId,
                };

                await _bus.Publish(newInMessage);
            }
            catch (OperationCanceledException)
            {
                // ignore on shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish incoming message {MessageId}", e.Id);
            }
        }
    }
}

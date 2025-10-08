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
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;

        public SendMessageService(ILogger<SendMessageService> logger, IConfiguration configuration, IBus bus)
        {
            _logger = logger;
            _configuration = configuration;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(SendMessageService)}.{nameof(ExecuteAsync)}() -> Background service is starting.");

            var bots = _configuration.GetSection(nameof(TelegramBotOptionsList)).Get<List<TelegramBotOptions>>();

            if (bots is null || bots.Count == 0)
            {
                _logger.LogWarning($"{nameof(SendMessageService)}.{nameof(ExecuteAsync)}() -> No bot configurations found.");
                await StopAsync(stoppingToken);
                return;
            }

            var startTasks = new List<Task>();
            for (int index = 0; index < bots.Count; index++)
            {
                var cfg = bots[index];
                var channelId = index + 1;
                startTasks.Add(StartBotAsync(cfg, channelId, stoppingToken));
            }

            await Task.WhenAll(startTasks);
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

        private async Task StartBotAsync(TelegramBotOptions cfg, int channelId, CancellationToken ct)
        {
            try
            {
                var telegramBot = new TelegramBotService();
                telegramBot.ChannelId = channelId;
                telegramBot.SetTokenAndName(cfg.Token, cfg.Name);
                telegramBot.DefineTelegramBotClient();

                telegramBot.OnIncomingMessage += (sender, args) =>
                {
                    _ = HandleIncomingMessageAsync(args, ct);
                };

                _logger.LogInformation("Starting bot {BotName} (ChannelId {ChannelId})", telegramBot.Name, channelId);

                _telegramBots[channelId] = telegramBot;
                await telegramBot.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start bot {ChannelId}", channelId);
            }
        }

        private async Task HandleIncomingMessageAsync(MessageEventArgs e, CancellationToken ct)
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

                await _bus.Publish(newInMessage, ct);
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

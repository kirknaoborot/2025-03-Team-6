using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Application.Services
{
    public class SendMessageService : BackgroundService
    {
        private readonly List<TelegramBotService> _telegramBots = [];
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

            if (bots != null)
            {
                foreach (var bot in bots)
                {
                    var telegramBot = new TelegramBotService(bot.Token, bot.Name);

                    telegramBot.OnIncomingMessage += OnIncomingMessage;

                    _logger.LogInformation($"{nameof(SendMessageService)}.{nameof(ExecuteAsync)}() -> Start bot: name '{telegramBot.Name}'.");

                    _telegramBots.Add(telegramBot);
                    await telegramBot.Start();
                }
            }
            else
            {
                await StopAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(SendMessageService)}.{nameof(ExecuteAsync)}() -> Background service is stopping.");

            foreach (var bot in _telegramBots)
            {
                await bot.Stop();
            }
        }

        private async void OnIncomingMessage(object sender, MessageEventArgs e)
        {
            _logger.LogInformation($"{nameof(SendMessageService)}.{nameof(OnIncomingMessage)}() -> Received text '{e.Text}', message id '{e.Id}', send data '{e.SendData}'");

            var newInMEssage = new ClientMessageEvent
            {
                Id = e.Id,
                MessageText = e.Text,
                SendData = e.SendData,
                Priority = "high"
            };

            await _bus.Publish(newInMEssage);
        }
    }
}

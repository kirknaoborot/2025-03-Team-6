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
        private static readonly List<TelegramBotService> _telegramBots = [];
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
                    var telegramBot = new TelegramBotService();
					telegramBot.SetTokenAndName(bot.Token, bot.Name);
					telegramBot.DefineTelegramBotClient();

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

		public static bool TryGetTelegramBotByToken(string token, out TelegramBotService telegramBot)
		{
			Console.WriteLine($"=====> '{token}', bot count '{_telegramBots.Count}'");

			_telegramBots.ForEach(x => Console.WriteLine($"===> x.Name = '{x.Name}', '{x.Token}'"));

			telegramBot = _telegramBots.FirstOrDefault(x => x.Token == token);

			return telegramBot != null;
		}

		private async void OnIncomingMessage(object sender, MessageEventArgs e)
        {
            _logger.LogInformation($"{nameof(SendMessageService)}.{nameof(OnIncomingMessage)}() -> Received text '{e.Text}', message id '{e.Id}', send data '{e.SendData}', bot token '{e.BotToken}'");

            var newInMEssage = new ClientMessageEvent
            {
                Id = e.Id,
				UserId = e.UserId,
                MessageText = e.Text,
                SendData = e.SendData,
                Priority = "high",
				Channel = Infrastructure.Shared.Enums.ChannelType.Telegram,
				BotToken = e.BotToken,
			};

            await _bus.Publish(newInMEssage);
        }
    }
}

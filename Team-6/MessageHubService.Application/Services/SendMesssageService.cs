using MessageHubService.Domain.Entities;

namespace MessageHubService.Application.Services
{
    public class SendMesssageService : BackgroundService
    {
        private readonly List<TelegramBotService> _telegramBots = [];
        private readonly ILogger<SendMesssageService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;

        public SendMesssageService(ILogger<SendMesssageService> logger, IConfiguration configuration, IBus bus)
        {
            _logger = logger;
            _configuration = configuration;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(SendMesssageService)}.{nameof(ExecuteAsync)}() -> Background service is starting.");

            var bots = _configuration.GetSection(nameof(TelegramBotOptionsList)).Get<List<TelegramBotOptions>>();

            if (bots != null)
            {
                foreach (var bot in bots)
                {
                    var telegramBot = new TelegramBot(bot.Token, bot.Name);

                    telegramBot.OnIncomingMessage += OnIncomingMessage;

                    _logger.LogInformation($"{nameof(SendMesssageService)}.{nameof(ExecuteAsync)}() -> Start bot: name '{telegramBot.Name}'.");

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
            _logger.LogInformation($"{nameof(SendMesssageService)}.{nameof(ExecuteAsync)}() -> Background service is stopping.");

            foreach (var bot in _telegramBots)
            {
                await bot.Stop();
            }
        }

        private async void OnIncomingMessage(object sender, MessageEventArgs e)
        {
            _logger.LogInformation($"{nameof(SendMesssageService)}.{nameof(OnIncomingMessage)}() -> Received text '{e.Text}', message id '{e.Id}', send data '{e.SendData}'");

            var newInMEssage = new ClientMessage(e.Id, e.Text, e.SendData, "high");

            await _bus.Publish(newInMEssage);
        }
    }
}

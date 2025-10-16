using Infrastructure.Shared.Contracts;
using MessageHubService.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Application.Services
{
    public class BotManagerService : IBotManagerService
    {
        private readonly ILogger<BotManagerService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IMemoryCache _cache;

		public BotManagerService(ILogger<BotManagerService> logger, IServiceScopeFactory serviceScopeFactory, IMemoryCache cache)
        {
            _logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
			_cache = cache;
        }

		public async Task AddBot(ChannelEvent channelEvent)
		{
			_logger.LogInformation($"{nameof(BotManagerService)}.{nameof(AddBot)}() -> channel event is null {channelEvent is null}");

			await StartBotAsync(channelEvent);
		}

        public bool TryGetTelegramBot(int id, out IBot telegramBot)
		{
            return _cache.TryGetValue(id, out telegramBot);
		}

        private async Task StartBotAsync(ChannelEvent channelEvent)
        {
			try
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var bot = scope.ServiceProvider.GetRequiredService<IBot>();
				bot.CreateBot(channelEvent);
				await bot.StartAsync();
				_cache.Set(bot.GetHashCode(), bot);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to start bot {ChannelId}", channelEvent.Id);
			}
		}
    }
}

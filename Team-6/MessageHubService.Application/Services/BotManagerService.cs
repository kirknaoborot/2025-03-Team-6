using Infrastructure.Shared.Contracts;
using MessageHubService.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Application.Services;

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

	public bool TryGetBot(int id, out IBot? telegramBot)
	{
		return _cache.TryGetValue(id, out telegramBot);
	}

	public async Task CreateBotAsync(ChannelCommand channelEvent)
	{
		_logger.LogInformation($"{nameof(BotManagerService)}.{nameof(CreateBotAsync)}() -> Channel event is null {channelEvent is null}");

		try
		{
			if (channelEvent != null)
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var bot = scope.ServiceProvider.GetRequiredService<IBot>();

				await bot.CreateBotAsync(channelEvent);
				await bot.StartAsync();
				_cache.Set(bot.GetHashCode(), bot);
			}
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, $"{nameof(BotManagerService)}.{nameof(CreateBotAsync)}() -> Failed to start bot {channelEvent?.Id}");
		}
	}

	public async Task UpdateBotAsync(ChannelCommand channelEvent)
	{
		_logger.LogInformation($"{nameof(BotManagerService)}.{nameof(UpdateBotAsync)}() -> Channel event is null {channelEvent is null}");

		try
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var bot = scope.ServiceProvider.GetRequiredService<IBot>();

			if (channelEvent != null)
			{
				if (_cache.TryGetValue<IBot>(channelEvent.Id, out var findedBot))
				{
					if (findedBot != null)
					{
						await findedBot.StopAsync();
						findedBot.Dispose();
						_cache.Remove(channelEvent.Id);
						await bot.CreateBotAsync(channelEvent);
						await bot.StartAsync();
						_cache.Set(bot.GetHashCode(), bot);
					}
				}
				else
				{
					await bot.CreateBotAsync(channelEvent);
					await bot.StartAsync();
					_cache.Set(bot.GetHashCode(), bot);
				}
			}
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, $"{nameof(BotManagerService)}.{nameof(UpdateBotAsync)}() -> Failed to start bot {channelEvent?.Id}");
		}
	}

	public async Task DeleteBotAsync(ChannelCommand channelEvent)
	{
		_logger.LogInformation($"{nameof(BotManagerService)}.{nameof(DeleteBotAsync)}() -> Channel event is null {channelEvent is null}");

		try
		{
			if (channelEvent != null)
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var bot = scope.ServiceProvider.GetRequiredService<IBot>();

				if (_cache.TryGetValue<IBot>(channelEvent.Id, out var findedBot))
				{
					if (findedBot != null)
					{
						await findedBot.StopAsync();
						findedBot.Dispose();
						_cache.Remove(channelEvent.Id);
					}
				}
			}
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, $"{nameof(BotManagerService)}.{nameof(DeleteBotAsync)}() -> Failed to start bot {channelEvent.Id}");
		}
	}
}
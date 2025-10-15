using ChannelSettings.Core.IRepositories;
using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace ChannelSettings.Application.Services
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly IBus _bus;
		private readonly IServiceProvider _serviceProvider;

		public Worker(ILogger<Worker> logger, IBus bus, IServiceProvider serviceProvider)
		{
			_logger = logger;
			_bus = bus;
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation($"{nameof(Worker)}.{nameof(ExecuteAsync)}() -> начало работы в {DateTimeOffset.Now}");

			using var scope = _serviceProvider.CreateScope();
			var channelService = scope.ServiceProvider.GetRequiredService<ChannelService>();

			var channelService1 = _serviceProvider.GetService<ChannelService>();

			if (channelService != null)
			{
				var channels = await channelService.GetAllAsync(CancellationToken.None);

				_logger.LogInformation($"{nameof(Worker)}.{nameof(ExecuteAsync)}() -> количество каналов {channels.Count()}");

				if (channels != null)
				{
					foreach (var i in channels)
					{
						var channelInfo = new ChannelEvent
						{
							Id = i.Id,
							Name = i.Name,
							Token = i.Token,
							Type = i.Type,
						};

						await _bus.Publish(channelInfo);
					}
				}
			}
		}
	}
}

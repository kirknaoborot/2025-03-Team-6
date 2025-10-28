using ChannelSettings.Core.IServices;
using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using MassTransit;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IBus bus, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _bus = bus;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var appName = _configuration["Serilog:Properties:Application"] ?? "Unknown Service";
            _logger.LogInformation("Starting up {@ApplicationName}", appName);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
            _logger.LogInformation($"{nameof(Worker)}.{nameof(ExecuteAsync)}() -> Getting Started at {DateTimeOffset.Now}");

			using var scope = _serviceProvider.CreateScope();
			var channelService = scope.ServiceProvider.GetRequiredService<IChannelService>();

			if (channelService != null)
			{
				var channels = await channelService.GetAllAsync(CancellationToken.None);

                _logger.LogInformation($"{nameof(Worker)}.{nameof(ExecuteAsync)}() -> Number of channels {channels.Count()}");

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
							Action = ChannelInfoAction.Create,
						};

						await _bus.Publish(channelInfo);
					}

					return;
				}
			}
		}
	}
}

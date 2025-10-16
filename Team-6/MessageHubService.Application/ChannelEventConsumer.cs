using MassTransit;
using Infrastructure.Shared.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MessageHubService.Application.Interfaces;
using Infrastructure.Shared.Enums;

namespace MessageHubService.Application
{
	public class ChannelEventConsumer : IConsumer<ChannelEvent>
	{
		private readonly ILogger<ChannelEventConsumer> _logger;
		private readonly IServiceProvider _serviceProvider;

		public ChannelEventConsumer(ILogger<ChannelEventConsumer> logger, IServiceProvider serviceProvider)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
		}

		public async Task Consume(ConsumeContext<ChannelEvent> context)
		{
			_logger.LogInformation($"{nameof(ChannelEventConsumer)}.{nameof(Consume)}() -> bot id '{context.Message.Id}', bot name '{context.Message.Name}', bot token '{context.Message.Token}', bot type '{context.Message.Type}', action '{context.Message.Action}'");

			using var scope = _serviceProvider.CreateScope();
			var sendMessageService = scope.ServiceProvider.GetRequiredService<IBotManagerService>();

			switch (context.Message.Action)
			{
				case ChannelInfoAction.Create:
					{
						await sendMessageService.CreateBotAsync(context.Message);
						break;
					}
				case ChannelInfoAction.Update:
					{
						await sendMessageService.UpdateBotAsync(context.Message);
						break;
					}
				case ChannelInfoAction.Delete:
					{
						await sendMessageService.DeleteBotAsync(context.Message);
						break;
					}
			}
		}
	}
}

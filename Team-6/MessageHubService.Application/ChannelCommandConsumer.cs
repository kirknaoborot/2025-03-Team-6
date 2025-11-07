using MassTransit;
using Infrastructure.Shared.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MessageHubService.Application.Interfaces;
using Infrastructure.Shared.Enums;

namespace MessageHubService.Application
{
	public class ChannelCommandConsumer : IConsumer<ChannelCommand>
	{
		private readonly ILogger<ChannelCommandConsumer> _logger;
		private readonly IServiceProvider _serviceProvider;

		public ChannelCommandConsumer(ILogger<ChannelCommandConsumer> logger, IServiceProvider serviceProvider)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
		}

		public async Task Consume(ConsumeContext<ChannelCommand> context)
		{
			_logger.LogInformation($"{nameof(ChannelCommandConsumer)}.{nameof(Consume)}() -> bot id '{context.Message.Id}', bot name '{context.Message.Name}', bot token '{context.Message.Token}', bot type '{context.Message.Type}', action '{context.Message.Action}'");

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

using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Application;

public class SendMessageEventConsumer : IConsumer<SendMessageEvent>
{
	private readonly ILogger<SendMessageEventConsumer> _logger;
	private readonly IServiceProvider _serviceProvider;

	public SendMessageEventConsumer(ILogger<SendMessageEventConsumer> logger , IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	public async Task Consume(ConsumeContext<SendMessageEvent> context)
	{
		_logger.LogInformation($"{nameof(SendMessageEventConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', message text '{context.Message.MessageText}', bot id '{context.Message.ChannelSettingsId}'");

		using var scope = _serviceProvider.CreateScope();
		var sendMessageService = scope.ServiceProvider.GetRequiredService<IBotManagerService>();

		if (sendMessageService.TryGetBot(context.Message.ChannelSettingsId, out var telegramBot))
		{
			var sendMessageDto = new SendMessageDto(context.Message.UserId, context.Message.MessageText);
			await telegramBot.SentMessageToClientAsync(sendMessageDto);
		}
	}
}

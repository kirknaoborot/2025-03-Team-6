using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Application.Services;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Application;

public class SendMessageEventConsumer : IConsumer<SendMessageEvent>
{
	private readonly ILogger<SendMessageEventConsumer> _logger;

	public SendMessageEventConsumer(ILogger<SendMessageEventConsumer> logger)
	{
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<SendMessageEvent> context)
	{
		_logger.LogInformation($"{nameof(SendMessageEventConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', message text '{context.Message.MessageText}', bot token '{context.Message.ChannelSettingsId}'");

		if (SendMessageService.TryGetTelegramBot(context.Message.ChannelSettingsId, out var telegramBot))
		{
			var sendMessageDto = new SendMessageDto(context.Message.UserId, context.Message.MessageText);

			await telegramBot.SentMessageToClient(sendMessageDto);
		}
	}
}

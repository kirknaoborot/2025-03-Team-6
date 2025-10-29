using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application;

internal class SendMessageEventConsumer : IConsumer<SendMessageEvent>
{
	private readonly IBus _bus;
	private readonly ILogger<SendMessageEventConsumer> _logger;

	public SendMessageEventConsumer(IBus bus, ILogger<SendMessageEventConsumer> logger)
	{
		_bus = bus;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<SendMessageEvent> context)
	{
		_logger.LogInformation($"{nameof(SendMessageEventConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', message text '{context.Message.MessageText}', channel '{context.Message.Channel}', channel id '{context.Message.ChannelSettingsId}'");

		var sendMessageCommand = new SendMessageCommand()
		{
			UserId = context.Message.UserId,
			MessageText = context.Message.MessageText,
			Channel = context.Message.Channel,
			ChannelSettingsId = context.Message.ChannelSettingsId,
		};

		await _bus.Publish(sendMessageCommand);
	}
}

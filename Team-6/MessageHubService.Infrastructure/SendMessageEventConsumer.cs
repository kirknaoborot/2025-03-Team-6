using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MessageHubService.Infrastructure;

public class SendMessageEventConsumer : IConsumer<SendMessageEvent>
{
	private readonly IBus _bus;
	private readonly IBotWork _botWork;
	private readonly ILogger<SendMessageEventConsumer> _logger;

	public SendMessageEventConsumer(IBus bus, IBotWork botWork, ILogger<SendMessageEventConsumer> logger)
	{
		_bus = bus;
		_botWork = botWork;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<SendMessageEvent> context)
	{
		_logger.LogInformation($"{nameof(SendMessageEventConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', message text '{context.Message.MessageText}'");

		var sendMessageDto = new SendMessageDto(context.Message.UserId, context.Message.MessageText);

        await _botWork.SentMessageToClientAsync(sendMessageDto);
	}
}

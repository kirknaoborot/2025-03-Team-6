using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application;

public class NotifySendEventConsumer : IConsumer<NotifySendEvent>
{
	private readonly IBus _bus;
	private readonly ILogger<NotifySendEventConsumer> _logger;

	public NotifySendEventConsumer(IBus bus, ILogger<NotifySendEventConsumer> logger)
	{
		_bus = bus;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<NotifySendEvent> context)
	{
		_logger.LogInformation($"{nameof(SendMessageEventConsumer)}.{nameof(Consume)}() -> agent id '{context.Message.AgentId}'");

		var sendMessageCommand = new NotifySendCommand()
		{
			AgentId = context.Message.AgentId,
		};

		await _bus.Publish(sendMessageCommand);
	}
}

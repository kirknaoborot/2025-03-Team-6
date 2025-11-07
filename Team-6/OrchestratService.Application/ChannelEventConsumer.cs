using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application;

public class ChannelEventConsumer : IConsumer<ChannelEvent>
{
	private readonly IBus _bus;
	private readonly ILogger<ChannelEventConsumer> _logger;

	public ChannelEventConsumer(IBus bus, ILogger<ChannelEventConsumer> logger)
	{
		_bus = bus;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<ChannelEvent> context)
	{
		_logger.LogInformation($"{nameof(SendMessageEventConsumer)}.{nameof(Consume)}() -> channel id '{context.Message.Id}', channel name '{context.Message.Name}', channel type '{context.Message.Type}', action '{context.Message.Action}'");

		var channelInfo = new ChannelCommand
		{
			Id = context.Message.Id,
			Name = context.Message.Name,
			Token = context.Message.Token,
			Type = context.Message.Type,
			Action = context.Message.Action,
		};

		await _bus.Publish(channelInfo);
	}
}

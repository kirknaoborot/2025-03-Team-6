using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application;

public class AgentStatusEventConsumer : IConsumer<AgentStatusEvent>
{
	private readonly IBus _bus;
	private readonly ILogger<AgentStatusEventConsumer> _logger;

	public AgentStatusEventConsumer(IBus bus, ILogger<AgentStatusEventConsumer> logger)
	{
		_bus = bus;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<AgentStatusEvent> context)
	{
		_logger.LogInformation($"{nameof(AgentStatusEventConsumer)}.{nameof(Consume)}() -> agent id '{context.Message.AgentId}', agent status '{context.Message.Status}', date time '{context.Message.Date}'");

		var agentStatus = new AgentStatusCommand
		{
			AgentId = context.Message.AgentId,
			Status = context.Message.Status,
			Date = context.Message.Date
		};

		await _bus.Publish(agentStatus);
	}
}

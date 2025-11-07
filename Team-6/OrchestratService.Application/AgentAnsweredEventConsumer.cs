using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application;

public class AgentAnsweredEventConsumer : IConsumer<AgentAnsweredEvent>
{
	private readonly IBus _bus;
	public readonly ILogger<AgentAnsweredEventConsumer> _logger;

	public AgentAnsweredEventConsumer(ILogger<AgentAnsweredEventConsumer> logger, IBus bus)
	{
		_logger = logger;
		_bus = bus;
	}

	public async Task Consume(ConsumeContext<AgentAnsweredEvent> context)
	{
		_logger.LogInformation($"{nameof(AgentAnsweredEventConsumer)}.{nameof(Consume)}() -> agent id '{context.Message.Id}'");

		var agentStatus = new AgentAnsweredCommand
		{
			Id = context.Message.Id
		};

		await _bus.Publish(agentStatus);
	}
}

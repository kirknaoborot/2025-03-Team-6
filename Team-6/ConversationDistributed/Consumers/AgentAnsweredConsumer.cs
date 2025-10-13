using ConversationDistributed.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationDistributed.Consumers;

public class AgentAnsweredConsumer : IConsumer<AgentAnsweredEvent>
{
	private readonly IAgentStateService _IUserStateService;
	private readonly ILogger<AgentAnsweredConsumer> _logger;

	public AgentAnsweredConsumer(IAgentStateService userStateService, ILogger<AgentAnsweredConsumer> logger)
	{ 
		_IUserStateService = userStateService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<AgentAnsweredEvent> context)
	{
		_logger.LogInformation($"{nameof(AgentAnsweredConsumer)}.{nameof(Consume)}() -> user id '{context.Message.Id}'");

		_IUserStateService.ReleaseConversationFromUser(context.Message.Id);
	}
}

using ConversationDistributed.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationDistributed.Consumers;

public class AgentAnsweredCommandConsumer : IConsumer<AgentAnsweredCommand>
{
	private readonly IAgentStateService _IUserStateService;
	private readonly ILogger<AgentAnsweredCommandConsumer> _logger;

	public AgentAnsweredCommandConsumer(IAgentStateService userStateService, ILogger<AgentAnsweredCommandConsumer> logger)
	{ 
		_IUserStateService = userStateService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<AgentAnsweredCommand> context)
	{
		_logger.LogInformation($"{nameof(AgentAnsweredCommandConsumer)}.{nameof(Consume)}() -> user id '{context.Message.Id}'");

		_IUserStateService.ReleaseConversationFromUser(context.Message.Id);
	}
}

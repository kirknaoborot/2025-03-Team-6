using ConversationDistributed.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationDistributed.Consumers
{
    public class AgentStatusCommandConsumer : IConsumer<AgentStatusCommand>
    {
        private readonly IAgentStateService _userState;
        private readonly ILogger<AgentStatusCommandConsumer> _logger;

        public AgentStatusCommandConsumer(IAgentStateService userState, ILogger<AgentStatusCommandConsumer> logger)
        {
            _userState = userState;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<AgentStatusCommand> context)
        {
            var msg = context.Message;
            _userState.UserUpdateState(msg);
            _logger.LogInformation($"{nameof(AgentStatusCommandConsumer)}.{nameof(Consume)}() -> User logged in: {msg.AgentId}");

            return Task.CompletedTask;
        }
    }
}

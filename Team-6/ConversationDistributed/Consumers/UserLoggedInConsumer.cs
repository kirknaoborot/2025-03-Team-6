using ConversationDistributed.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationDistributed.Consumers
{
    public class UserLoggedInConsumer : IConsumer<UserLoggedInEvent>
    {
        private readonly IAgentStateService _userState;
        private readonly ILogger<UserLoggedInConsumer> _logger;

        public UserLoggedInConsumer(IAgentStateService userState, ILogger<UserLoggedInConsumer> logger)
        {
            _userState = userState;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<UserLoggedInEvent> context)
        {
            var msg = context.Message;
            _userState.UserLoggedIn(msg);
            _logger.LogInformation($"Пользователь вошёл: {msg.FullName} ({msg.Login})");

            return Task.CompletedTask;
        }
    }
}

using ConversationDistributed.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationDistributed.Consumers;

public class DefineOperatorForConversationConsumer : IConsumer<DefineOperatorForConversationCommand>
{
	private readonly IBus _bus;
	private readonly IAgentStateService _IUserStateService;
	private readonly ILogger<DefineOperatorForConversationConsumer> _logger;

	public DefineOperatorForConversationConsumer(IAgentStateService userStateService, IBus bus, ILogger<DefineOperatorForConversationConsumer> logger)
	{
		_IUserStateService = userStateService;
		_bus = bus;
		_logger = logger;
	}

	public Task Consume(ConsumeContext<DefineOperatorForConversationCommand> context)
	{
		var user = _IUserStateService.GetFirstFreeOperator();

		_logger.LogInformation($"{nameof(DefineOperatorForConversationConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', conversation id '{context.Message.ConversationId}', channel '{context.Message.Channel}', user is finded '{user != null}'");

		if (user != null)
		{
			_IUserStateService.AssignConversationToUser(user.Id, context.Message.ConversationId);
		}

		var workerId = user == null ? Guid.Empty : user.Id;

		var defineOperatorForConversationEvent = new DefineAgentEvent
		{
			ConversationId = context.Message.ConversationId,
			UserId = context.Message.UserId,
			WorkerId = workerId,
			MessageText = context.Message.MessageText,
			CreateDate = context.Message.CreateDate,
			Channel = context.Message.Channel,
			BotToken = context.Message.BotToken
		};

		_bus.Publish(defineOperatorForConversationEvent);

		return Task.CompletedTask;
	}
}

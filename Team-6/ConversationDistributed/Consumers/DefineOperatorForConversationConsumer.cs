using ConversationDistributed.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationDistributed.Consumers;

public class DefineOperatorForConversationConsumer : IConsumer<DefineOperatorForConversationCommand>
{
	private readonly IBus _bus;
	private readonly IAgentStateService _IUserStateService;
	private readonly ILogger<DefineOperatorForConversationConsumer> _logger;
	private readonly ConvState _convState;

	public DefineOperatorForConversationConsumer(IAgentStateService userStateService, IBus bus, ILogger<DefineOperatorForConversationConsumer> logger, ConvState convState)
	{
		_IUserStateService = userStateService;
		_bus = bus;
		_logger = logger;
		_convState = convState;
	}

	public async Task Consume(ConsumeContext<DefineOperatorForConversationCommand> context)
	{
		var user = _IUserStateService.GetFirstFreeOperator();

		_logger.LogInformation($"{nameof(DefineOperatorForConversationConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', conversation id '{context.Message.ConversationId}', channel '{context.Message.Channel}', user is finded '{user != null}'");

		if (user != null)
		{
			_IUserStateService.AssignConversationToUser(user.Id, context.Message.ConversationId);

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

			await _bus.Publish(defineOperatorForConversationEvent);
		}
		else
		{
			await _convState.AddConversationAsync(context.Message);
		}
	}
}

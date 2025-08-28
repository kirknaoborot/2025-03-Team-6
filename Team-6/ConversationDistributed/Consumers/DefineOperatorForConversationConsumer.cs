using ConversationDistributed.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationDistributed.Consumers;

public class DefineOperatorForConversationConsumer : IConsumer<DefineOperatorForConversationCommand>
{
	private readonly IBus _bus;
	private readonly IAgentStateService _IUserStateService;

	public DefineOperatorForConversationConsumer(IAgentStateService userStateService, IBus bus)
	{
		_IUserStateService = userStateService;
		_bus = bus;
	}

	public Task Consume(ConsumeContext<DefineOperatorForConversationCommand> context)
	{
		Console.WriteLine($"====> DefineOperatorForConversationConsumer Consume => ConversationId = '{context.Message.ConversationId}', MessageText = '{context.Message.MessageText}'");

		var user = _IUserStateService.GetFirstFreeOperator();
		Console.WriteLine($"====> DefineOperatorForConversationConsumer Consume => ConversationId = '{user != null}'");
		if (user != null)
		{
			_IUserStateService.AssignConversationToUser(user.Id, context.Message.ConversationId);

			var defineOperatorForConversationEvent = new DefineAgentEvent
			{
				ConversationId = context.Message.ConversationId,
				WorkerId = user.Id,
				MessageText = context.Message.MessageText,
				CreateDate = context.Message.CreateDate,
				Channel = context.Message.Channel
			};

			_bus.Publish(defineOperatorForConversationEvent);
		}

		return Task.CompletedTask;
	}
}

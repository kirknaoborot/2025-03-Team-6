using Infrastructure.Shared.Contracts;
using MassTransit;

namespace OrchestratService.Application;

public class DefineAgentConsumer : IConsumer<DefineAgentEvent>
{
	private readonly IBus _bus;

	public DefineAgentConsumer(IBus bus)
	{
		_bus = bus;
	}

	public Task Consume(ConsumeContext<DefineAgentEvent> context)
	{
		var updateConversationCommand = new ConversationCommand()
		{
			ConversationId = context.Message.ConversationId,
			Message = context.Message.MessageText,
			Status = Infrastructure.Shared.Enums.StatusType.Distributed,
			Channel = context.Message.Channel,
			WorkerId = context.Message.WorkerId,
			CreateDate = context.Message.CreateDate,
		};

		return _bus.Publish(updateConversationCommand);
	}
}

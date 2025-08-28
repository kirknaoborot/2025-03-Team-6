using Infrastructure.Shared.Contracts;
using MassTransit;

namespace OrchestratService.Application
{
	public class ConversationEventConsumer : IConsumer<ConversationEvent>
	{
		private readonly IBus _bus;

		public ConversationEventConsumer(IBus bus)
		{
			_bus = bus;
		}

		public async Task Consume(ConsumeContext<ConversationEvent> context)
		{
			var defineOperatorForConversationCommand = new DefineOperatorForConversationCommand
			{
				ConversationId = context.Message.ConversationId,
				MessageText = context.Message.Message,
				CreateDate = context.Message.CreateDate,
				Channel = context.Message.Channel,
			};

			await _bus.Publish(defineOperatorForConversationCommand);
		}
	}
}

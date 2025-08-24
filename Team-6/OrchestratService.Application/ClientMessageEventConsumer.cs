using Infrastructure.Shared.Contracts;
using MassTransit;
using Infrastructure.Shared.Enums;

namespace OrchestratService.Application
{
    public class ClientMessageEventConsumer : IConsumer<ClientMessageEvent>
    {
		private readonly IBus _bus;

		public ClientMessageEventConsumer(IBus bus)
		{
			_bus = bus;
		}

		public async Task Consume(ConsumeContext<ClientMessageEvent> context)
        {
            Console.WriteLine($"message id {context.Message.Id}, message text {context.Message.MessageText}");

			var createConversationCommand = new CreateConversationCommand
			{
				ConversationId = new Guid(),
				Message = context.Message.MessageText,
				Status = StatusType.New,
				Channel = ChannelType.Telegram,
				CreateDate = DateTimeOffset.UtcNow,
			};

			await _bus.Publish(createConversationCommand);
        }
    }
}

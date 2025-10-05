using Infrastructure.Shared.Contracts;
using MassTransit;
using Infrastructure.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application
{
    public class ClientMessageEventConsumer : IConsumer<ClientMessageEvent>
    {
		private readonly IBus _bus;
		private readonly ILogger<ClientMessageEventConsumer> _logger;

		public ClientMessageEventConsumer(IBus bus, ILogger<ClientMessageEventConsumer> logger)
		{
			_bus = bus;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ClientMessageEvent> context)
        {
			_logger.LogInformation($"{nameof(ClientMessageEventConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', message text '{context.Message.MessageText}', channel '{context.Message.Channel}', bot id '{context.Message.ChannelSettingsId}'");

			var createConversationCommand = new ConversationCommand
			{
				ConversationId = Guid.NewGuid(),
				UserId = context.Message.UserId,
				Message = context.Message.MessageText,
				Status = StatusType.New,
				Channel = context.Message.Channel,
				CreateDate = context.Message.SendData,
                ChannelSettingsId = context.Message.ChannelSettingsId,
			};

			await _bus.Publish(createConversationCommand);
        }
    }
}

using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application
{
	public class ConversationEventConsumer : IConsumer<ConversationEvent>
	{
		private readonly IBus _bus;
		private readonly ILogger<ConversationEventConsumer> _logger;

		public ConversationEventConsumer(IBus bus, ILogger<ConversationEventConsumer> logger)
		{
			_bus = bus;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ConversationEvent> context)
		{
			_logger.LogInformation($"{nameof(ConversationEventConsumer)}.{nameof(Consume)}() -> user id '{context.Message.UserId}', conversation id '{context.Message.ConversationId}', channel '{context.Message.Channel}'");

			var defineOperatorForConversationCommand = new DefineOperatorForConversationCommand
			{
				ConversationId = context.Message.ConversationId,
				UserId = context.Message.UserId,
				MessageText = context.Message.Message,
				CreateDate = context.Message.CreateDate,
				Channel = context.Message.Channel,
				ChannelSettingsId = context.Message.ChannelSettingsId,
			};

			await _bus.Publish(defineOperatorForConversationCommand);
		}
	}
}

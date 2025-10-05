using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrchestratService.Application;

public class DefineAgentConsumer : IConsumer<DefineAgentEvent>
{
	private readonly IBus _bus;
	private readonly ILogger<DefineAgentConsumer> _logger;

	public DefineAgentConsumer(IBus bus, ILogger<DefineAgentConsumer> logger)
	{
		_bus = bus;
		_logger = logger;
	}

	public Task Consume(ConsumeContext<DefineAgentEvent> context)
	{
		var status = context.Message.WorkerId == Guid.Empty ? Infrastructure.Shared.Enums.StatusType.AgentNotFound : Infrastructure.Shared.Enums.StatusType.Distributed;

		_logger.LogInformation($"{nameof(DefineAgentConsumer)}.{nameof(Consume)}() -> status '{status}'");

		var updateConversationCommand = new ConversationCommand()
		{
			ConversationId = context.Message.ConversationId,
			UserId = context.Message.UserId,
			Message = context.Message.MessageText,
			Status = status,
			Channel = context.Message.Channel,
			WorkerId = context.Message.WorkerId,
			CreateDate = context.Message.CreateDate,
			ChannelSettingsId = context.Message.ChannelSettingsId,
			Answer = context.Message.Answer,
		};

		return _bus.Publish(updateConversationCommand);
	}
}

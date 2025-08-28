using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using MassTransit;

namespace ConversationService.Infrastructure.Messaging.Consumers;

public class ConversationConsumer : IConsumer<ConversationCommand>
{
	private readonly IBus _bus;
	private readonly IConversationService _service;

    public ConversationConsumer(IConversationService service, IBus bus)
    {
		_bus = bus;
		_service = service;
    }

    public async Task Consume(ConsumeContext<ConversationCommand> context)
    {
		var cmd = context.Message;

		var dto = new ConversationDto
		{
			ConversationId = cmd.ConversationId,
			Message = cmd.Message,
			Channel = cmd.Channel,
			Status = cmd.Status,
			WorkerId = cmd.WorkerId,
			CreateDate = cmd.CreateDate
		};

		if (cmd.Status == StatusType.New)
		{
			await _service.CreateConversation(dto);

			var createConversationCommand = new ConversationEvent
			{
				ConversationId = cmd.ConversationId,
				Message = cmd.Message,
				Channel = cmd.Channel,
				Status = cmd.Status,
				WorkerId = cmd.WorkerId,
				CreateDate = cmd.CreateDate
			};

			await _bus.Publish(createConversationCommand);
		}
		else
		{
			await _service.UpdateConversation(dto);
		}
	}
}
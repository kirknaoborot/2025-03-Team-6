using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationService.Infrastructure.Messaging.Consumers;

public class CreateConversationConsumer : IConsumer<CreateConversationCommand>
{
	private readonly IBus _bus;
	private readonly IConversationService _service;

    public CreateConversationConsumer(IConversationService service, IBus bus)
    {
		_bus = bus;
		_service = service;
    }

    public async Task Consume(ConsumeContext<CreateConversationCommand> context)
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

        await _service.CreateConversation(dto);

		var createConversationCommand = new CreateConversationEvent
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
}
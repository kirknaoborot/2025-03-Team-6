using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ConversationService.Infrastructure.Messaging.Consumers;

public class ConversationConsumer : IConsumer<ConversationCommand>
{
	private readonly IBus _bus;
	private readonly IConversationService _service;
	private readonly ILogger<ConversationConsumer> _logger;

	public ConversationConsumer(IConversationService service, IBus bus, ILogger<ConversationConsumer> logger)
    {
		_bus = bus;
		_service = service;
		_logger = logger;
    }

    public async Task Consume(ConsumeContext<ConversationCommand> context)
    {
		var cmd = context.Message;

		_logger.LogInformation($"{nameof(ConversationConsumer)}.{nameof(Consume)}() -> user id '{cmd.UserId}', conversation id '{cmd.ConversationId}', status '{cmd.Status}', bot id '{cmd.ChannelSettingsId}'");

		var dto = new ConversationDto
		{
			ConversationId = cmd.ConversationId,
			Message = cmd.Message,
			Channel = cmd.Channel,
			Status = cmd.Status,
			WorkerId = cmd.WorkerId,
			CreateDate = cmd.CreateDate,
			UserId = cmd.UserId,
			Answer = cmd.Answer,
			ChannelSettingsId = cmd.ChannelSettingsId
		};

		if (cmd.Status == StatusType.New)
		{
			await _service.CreateConversation(dto);

			var sendMessageEvent = new SendMessageEvent
			{
				UserId = cmd.UserId,
				MessageText = "Ваше обращение обрабатывается. Пожалуйста, дождитесь ответа оператора",
				Channel = cmd.Channel,
				ChannelSettingsId = cmd.ChannelSettingsId,
			};

			await _bus.Publish(sendMessageEvent);

			var createConversationCommand = new ConversationEvent
			{
				ConversationId = cmd.ConversationId,
				UserId = cmd.UserId,
				Message = cmd.Message,
				Channel = cmd.Channel,
				Status = cmd.Status,
				WorkerId = cmd.WorkerId,
				CreateDate = cmd.CreateDate,
                ChannelSettingsId = cmd.ChannelSettingsId,
			};

			await _bus.Publish(createConversationCommand);
		}
		else if (cmd.Status == StatusType.AgentNotFound)
		{
			await _service.UpdateConversation(dto);

			var sendMessageEvent = new SendMessageEvent
			{
				UserId = cmd.UserId,
				MessageText = cmd.Answer,
				Channel = cmd.Channel,
                ChannelSettingsId = cmd.ChannelSettingsId,
			};

			await _bus.Publish(sendMessageEvent);
		}
		else
		{
			await _service.UpdateConversation(dto);
		}
	}
}
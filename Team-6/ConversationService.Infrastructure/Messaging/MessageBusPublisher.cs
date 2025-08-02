using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using ConversationService.Infrastructure.Messaging.Events;
using MassTransit;

namespace ConversationService.Infrastructure.Messaging;

public class MessageBusPublisher : IMessageBusPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MessageBusPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishConversationCreated(ConversationDto dto)
    {
        await _publishEndpoint.Publish(new ConversationCreated
        {
            ConversationId = dto.ConversationId,
            Channel = dto.Channel.ToString(),
            Message = dto.Message,
            Status = dto.Status.ToString(),
            WorkerId = dto.WorkerId,
            CreateDate = dto.CreateDate
        });
    }
}
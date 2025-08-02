using ConversationService.Application.DTO;

namespace ConversationService.Application.Interfaces;

public interface IMessageBusPublisher
{
    Task PublishConversationCreated(ConversationDto conversation);
}
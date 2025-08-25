using ConversationService.Application.DTO;

namespace ConversationService.Application.Interfaces;

public interface IConversationService
{
    Task<IReadOnlyCollection<ConversationDto>> GetAllConversations();
    Task<ConversationDto> GetConversation(Guid conversationId);
    Task CreateConversation(ConversationDto dto);
}
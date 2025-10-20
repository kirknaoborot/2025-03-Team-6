using ConversationService.Application.DTO;
using Infrastructure.Shared.Enums;

namespace ConversationService.Application.Interfaces;

public interface IConversationService
{
    Task<IReadOnlyCollection<ConversationDto>> GetAllConversations(Guid userId, RoleType role);
    Task<ConversationDto> GetConversation(Guid conversationId);
    Task CreateConversation(ConversationDto dto);
	Task UpdateConversation(ConversationDto dto);
    Task ReplyConversation(Guid conversationId, string messageAnswer);
    Task<StatisticsDto> GetStatistics();
}
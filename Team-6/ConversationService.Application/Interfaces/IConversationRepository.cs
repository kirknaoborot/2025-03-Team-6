using ConversationService.Domain.Entities;

namespace ConversationService.Application.Interfaces;

public interface IConversationRepository
{
    Task<List<Conversation>> GetConversations();
    Task<Conversation> GetConversation(Guid id);
    Task CreateConversation(Conversation conversation);
	Task UpdateConversation(Conversation conversation);
    Task<(int total, int answered, int inWork, int withoutAnswer)> GetStatistics();
}
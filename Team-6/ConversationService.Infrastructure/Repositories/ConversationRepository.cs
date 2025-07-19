using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using ConversationService.Domain.Entities;
using ConversationService.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ConversationService.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ConversationServiceContext _context;

    public ConversationRepository(ConversationServiceContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Метод получения обращений
    /// </summary>
    /// <returns></returns>
    public async Task<List<Conversation>> GetConversations()
    {
        var conversations = await _context.Conversations
            .TagWith("Запрос получения списка пользователей")
            .ToListAsync();

        return conversations;
    }

    /// <summary>
    /// Метод получения обращения по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Conversation> GetConversation(Guid id)
    {
        var conversation = await _context.Conversations
            .TagWith($"Запрос получения пользователя по идентификатору: {id}")
            .SingleOrDefaultAsync(x => x.ConversationId == id);
        
        return conversation;
    }
}
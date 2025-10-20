using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using ConversationService.Domain.Entities;
using ConversationService.Infrastructure.DbContext;
using Infrastructure.Shared.Enums;
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
            .AsNoTracking()
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
            .AsNoTracking()
            .TagWith($"Запрос получения пользователя по идентификатору: {id}")
            .SingleOrDefaultAsync(x => x.ConversationId == id);
        
        return conversation;
    }

    /// <summary>
    /// Метод создния обращения
    /// </summary>
    /// <param name="conversation"></param>
    public async Task CreateConversation(Conversation conversation)
    {

		await _context.Conversations.AddAsync(conversation);
        await _context.SaveChangesAsync();
    }


	/// <summary>
	/// Метод обновления обращения
	/// </summary>
	/// <param name="conversation"></param>
	public async Task UpdateConversation(Conversation conversation)
	{
		_context.Conversations.Update(conversation);
		await _context.SaveChangesAsync();
	}

        /// <summary>
        /// Возвращает агрегированные статистические показатели по обращениям
        /// </summary>
        public async Task<(int total, int answered, int inWork, int withoutAnswer)> GetStatistics()
        {
            var query = _context.Conversations.AsNoTracking();

            var total = await query.CountAsync();
            var answered = await query.CountAsync(x => x.Status == StatusType.Closed);
            var inWork = await query.CountAsync(x => x.Status == StatusType.InWork || x.Status == StatusType.Distributed);
            var withoutAnswer = await query.CountAsync(x => x.Status == StatusType.New || x.Status == StatusType.AgentNotFound);

            return (total, answered, inWork, withoutAnswer);
        }
}
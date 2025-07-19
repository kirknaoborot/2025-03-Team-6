using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using ConversationService.Domain.Entities;

namespace ConversationService.Application.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;

    public ConversationService(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    /// <summary>
    /// Метод получения списка обращений
    /// </summary>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ConversationDto>> GetAllConversations()
    {
        var conversations = await _conversationRepository.GetConversations();

        var result = conversations
            .Select(x => new ConversationDto
            {
                ConversationId = x.ConversationId,
                Channel = x.Channel,
                Message = x.Message,
                Status = x.Status,
                WorkerId = x.WorkerId,
                CreateDate = x.CreateDate
            })
            .ToList();
        
        return result;
    }

    /// <summary>
    /// Метод получения идентификатора по идентификатору
    /// </summary>
    /// <param name="conversationId"></param>
    /// <returns></returns>
    public async Task<ConversationDto> GetConversation(Guid conversationId)
    {
        var conversation = await _conversationRepository.GetConversation(conversationId);

        var result = conversation is null
            ? null
            : new ConversationDto
            {
                ConversationId = conversationId,
                Channel = conversation.Channel,
                Message = conversation.Message,
                Status = conversation.Status,
                WorkerId = conversation.WorkerId,
                CreateDate = conversation.CreateDate
            };
        
        return result;
    }
}
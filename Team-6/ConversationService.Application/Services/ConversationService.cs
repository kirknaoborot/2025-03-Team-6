using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using ConversationService.Domain.Entities;
using Infrastructure.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Threading.Channels;

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
    public async Task<IReadOnlyCollection<ConversationDto>> GetAllConversations(Guid userId, RoleType role)
    {
        var conversations = await _conversationRepository.GetConversations();

        if (role == RoleType.Worker)
        {
            conversations = conversations
                .Where(x => x.WorkerId ==  userId)
                .ToList();
        }

        var result = conversations
            .Select(x => new ConversationDto
            {
                ConversationId = x.ConversationId,
                Channel = x.Channel,
                Message = x.Message,
                Status = x.Status,
                WorkerId = x.WorkerId,
                CreateDate = x.CreateDate,
                Number = $"{x.PrefixNumber}{x.Number}"
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
                CreateDate = conversation.CreateDate,
                Answer = conversation.Answer,
                Number = $"{conversation.PrefixNumber}{conversation.Number}",
                UserId = conversation.UserId ?? 0
            };
        
        return result;
    }

    public async Task CreateConversation(ConversationDto dto)
    {
        var conversation = new Conversation
        {
            ConversationId = dto.ConversationId,
            Channel = dto.Channel,
            Message = dto.Message,
            Status = dto.Status,
            WorkerId = dto.WorkerId,
            CreateDate = dto.CreateDate,
            UserId = dto.UserId,
        };
        
        await _conversationRepository.CreateConversation(conversation);
    }

	public async Task UpdateConversation(ConversationDto dto)
	{
		var conversation = new Conversation
		{
			ConversationId = dto.ConversationId,
			Channel = dto.Channel,
			Message = dto.Message,
			Status = dto.Status,
			WorkerId = dto.WorkerId,
			CreateDate = dto.CreateDate,
            Answer = dto.Answer,
            UserId = dto.UserId,
        };

		await _conversationRepository.UpdateConversation(conversation);
	}

    public async Task UpdateConversation(Conversation conversation)
    {
        await _conversationRepository.UpdateConversation(conversation);
    }

    public async Task ReplyConversation(Guid conversationId, string messageAnswer)
    {
        var conversation = await GetConversation(conversationId);

        conversation.WorkerId = conversation.WorkerId;
        conversation.CreateDate = conversation.CreateDate;
        conversation.Message = conversation.Message;
        conversation.Channel = conversation.Channel;
        conversation.Answer = messageAnswer;
        conversation.Status = StatusType.Closed;
        conversation.Number = conversation.Number;
        conversation.UserId = conversation.UserId;

        await UpdateConversation(conversation);
    }
}
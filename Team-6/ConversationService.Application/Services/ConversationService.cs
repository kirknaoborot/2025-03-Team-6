using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using ConversationService.Domain.Entities;
using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using MassTransit;

namespace ConversationService.Application.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IBus _bus;

    public ConversationService(IConversationRepository conversationRepository, IBus bus)
    {
        _conversationRepository = conversationRepository;
        _bus = bus;
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
                UserId = conversation.UserId ?? 0,
                ChannelSettingsId = conversation.ChannelSettingsId
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
            ChannelSettingsId = dto.ChannelSettingsId
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
            ChannelSettingsId = dto.ChannelSettingsId
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
        conversation.ChannelSettingsId = conversation.ChannelSettingsId;

        await UpdateConversation(conversation);

        var sendMessageEvent = new SendMessageEvent
        {
           MessageText = conversation.Answer,
           UserId = conversation.UserId,
           Channel = conversation.Channel,
           ChannelSettingsId = conversation.ChannelSettingsId
        };

        await _bus.Publish(sendMessageEvent);

		var agentAnsweredEvent = new AgentAnsweredEvent
		{
			Id = conversation.WorkerId
		};

		await _bus.Publish(agentAnsweredEvent);
	}
}
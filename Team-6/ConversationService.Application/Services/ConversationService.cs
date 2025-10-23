using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using ConversationService.Domain.Entities;
using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ConversationService.Application.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IBus _bus;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(IConversationRepository conversationRepository, IBus bus, ILogger<ConversationService> logger)
    {
        _conversationRepository = conversationRepository;
        _bus = bus;
        _logger = logger;
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
            .OrderByDescending(x => x.CreateDate)
            .ToList();
        _logger.LogInformation($"Получено обращений: {conversations.Count}");
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
        _logger.LogInformation($"Получено обращение: {conversation.ConversationId}");
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
        _logger.LogInformation($"Создано новое обращение: {conversation.ConversationId}");
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
        _logger.LogInformation($"Обновлено обращение: {conversation.ConversationId}");
    }

    public async Task UpdateConversation(Conversation conversation)
    {
        await _conversationRepository.UpdateConversation(conversation);
    }

    public async Task ReplyConversation(Guid conversationId, string messageAnswer)
    {
        _logger.LogInformation("Начата обработка ответа на обращение {ConversationId}", conversationId);
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
        _logger.LogDebug("Опубликовано событие SendMessageEvent для пользователя {UserId} в канале {Channel}", conversation.UserId, conversation.Channel);

        var agentAnsweredEvent = new AgentAnsweredEvent
		{
			Id = conversation.WorkerId
		};

		await _bus.Publish(agentAnsweredEvent);
        _logger.LogDebug("Опубликовано событие AgentAnsweredEvent для оператора {AgentId}", conversation.WorkerId);
    }

    public async Task<StatisticsDto> GetStatistics()
    {
        var (total, answered, inWork, withoutAnswer) = await _conversationRepository.GetStatistics();
        return new StatisticsDto
        {
            Total = total,
            Answered = answered,
            InWork = inWork,
            WithoutAnswer = withoutAnswer
        };
    }

    public async Task<IReadOnlyCollection<DailyStatDto>> GetDailyStatistics(DateOnly from, DateOnly to)
    {
        var raw = await _conversationRepository.GetDailyStatistics(from, to);
        return raw
            .Select(x => new DailyStatDto { Date = x.date, Total = x.total })
            .ToList();
    }
}
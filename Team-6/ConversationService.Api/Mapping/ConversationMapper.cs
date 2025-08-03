using ConversationService.Api.Mapping.Responses;
using ConversationService.Application.DTO;

namespace ConversationService.Api.Mapping;

public static class ConversationMapper
{
    public static ConversationResponse ToResponse(ConversationDto conversationDto)
    {
        return new ConversationResponse
        {
            ConversationId = conversationDto.ConversationId,
            Channel = conversationDto.Channel.ToString(),
            Message = conversationDto.Message,
            Status = conversationDto.Status.ToString(),
            CreateDate = conversationDto.CreateDate,
            WorkerId = conversationDto.WorkerId,
        };
    }
}
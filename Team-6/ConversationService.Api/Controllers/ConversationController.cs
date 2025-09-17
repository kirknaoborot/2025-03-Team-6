using ConversationService.Api.Mapping;
using ConversationService.Api.Models.Requests;
using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using Infrastructure.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConversationService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    /// <summary>
    /// Метод получения списка обращений
    /// </summary>
    /// <returns></returns>
    [HttpGet("conversations")]
    public async Task<IActionResult> Get()
    {
        var requestUser = HttpContext.User;

        var userId = Guid.TryParse(User.FindFirst("id")?.Value, out var resultUserId) ? resultUserId : Guid.Empty;
        var role = Enum.TryParse<RoleType>(User.FindFirst(ClaimTypes.Role)?.Value, true, out var resultEnum) ? resultEnum : RoleType.Worker;

        var conversations = await _conversationService.GetAllConversations(userId, role);

        var result = conversations
            .Select(x => ConversationMapper.ToResponse(x))
            .ToList();
        
        return Ok(result);
    }

    /// <summary>
    /// Метод получения обращения по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("conversation")]
    public async Task<ActionResult<ConversationDto>> GetConversation([FromQuery] Guid id)
    {
        var conversation = await _conversationService.GetConversation(id);

        if (conversation is null)
        {
            return NotFound();
        }

        var result = ConversationMapper.ToResponse(conversation);

        return Ok(result);
    }

    /// <summary>
    /// Метод закрытия обращения
    /// </summary>
    /// <param name="id"></param>
    /// <param name="reply"></param>
    /// <returns></returns>
    [HttpPost("conversation-reply")]
    public async Task<IActionResult> UpdateConversation([FromQuery] Guid id, [FromBody] ReplyDto reply)
    {
        await _conversationService.ReplyConversation(id, reply.AgentMessage);
        return Ok();
    }
}
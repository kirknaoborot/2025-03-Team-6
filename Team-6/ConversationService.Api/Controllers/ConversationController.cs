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
        // 1) Пытаемся взять из клеймов (если вдруг сервис будет с авторизацией)
        var userId = Guid.TryParse(User.FindFirst("id")?.Value, out var tmpUserId) ? tmpUserId : Guid.Empty;

        // ВАЖНО: в токене клейм называется "role", а не ClaimTypes.Role
        var roleStr = User.FindFirst("role")?.Value;


        if (userId == Guid.Empty && Request.Headers.TryGetValue("X-User-Id", out var xUserId))
            Guid.TryParse(xUserId.FirstOrDefault(), out userId);

        if (string.IsNullOrWhiteSpace(roleStr) && Request.Headers.TryGetValue("X-User-Role", out var xRole))
            roleStr = xRole.FirstOrDefault();

        // Парсим роль
        var role = Enum.TryParse<RoleType>(roleStr, true, out var parsedRole)
            ? parsedRole
            : RoleType.Worker;

        var conversations = await _conversationService.GetAllConversations(userId, role);

        var result = conversations.Select(ConversationMapper.ToResponse).ToList();
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
    [HttpPost("conversation-reply-close")]
    public async Task<IActionResult> UpdateConversation([FromQuery] Guid id, [FromBody] ReplyDto reply)
    {
        await _conversationService.ReplyConversation(id, reply.Message);
        return Ok();
    }
}
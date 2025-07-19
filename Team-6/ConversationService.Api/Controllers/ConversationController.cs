using ConversationService.Api.Mapping;
using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        var conversations = await _conversationService.GetAllConversations();

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
    [HttpGet("conversations/{id}")]
    public async Task<ActionResult<ConversationDto>> GetConversation(Guid id)
    {
        var conversation = await _conversationService.GetConversation(id);

        if (conversation is null)
        {
            return NotFound();
        }

        var result = ConversationMapper.ToResponse(conversation);

        return Ok(result);
    }
}
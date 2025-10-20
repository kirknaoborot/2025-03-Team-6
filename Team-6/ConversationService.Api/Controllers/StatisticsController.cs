using ConversationService.Application.DTO;
using ConversationService.Application.Interfaces;
using Infrastructure.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ConversationService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public StatisticsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    /// <summary>
    /// Возвращает агрегированную статистику обращений.
    /// Доступно только администраторам (проверка роли на фронте/гейтвее).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<StatisticsDto>> Get()
    {
        var stats = await _conversationService.GetStatistics();
        return Ok(stats);
    }
}



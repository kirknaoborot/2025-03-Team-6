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

    /// <summary>
    /// Статистика по дням (включительно).
    /// </summary>
    [HttpGet("daily")]
    public async Task<ActionResult<IReadOnlyCollection<DailyStatDto>>> GetDaily([FromQuery] DateOnly from, [FromQuery] DateOnly to)
    {
        if (to < from) return BadRequest("to must be >= from");
        var items = await _conversationService.GetDailyStatistics(from, to);
        return Ok(items);
    }
}



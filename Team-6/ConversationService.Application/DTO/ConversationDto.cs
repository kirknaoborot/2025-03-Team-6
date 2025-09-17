using Infrastructure.Shared.Enums;

namespace ConversationService.Application.DTO;

public class ConversationDto
{
    /// <summary>
    /// Идентификатор обращения
    /// </summary>
    public Guid ConversationId { get; set; }
    
    /// <summary>
    /// Канал
    /// </summary>
    public ChannelType Channel { get; set; }
    
    /// <summary>
    /// Сообщение обращения
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Статус /*отдельный класс*/
    /// </summary>
    public StatusType Status { get; set; }
    
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid WorkerId { get; set; }
    
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTimeOffset CreateDate { get; set; }

    /// <summary>
    /// Ответ оператора
    /// </summary>
    public string Answer { get; set; }
}
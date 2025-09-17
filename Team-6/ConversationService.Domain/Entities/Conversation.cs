using Infrastructure.Shared.Enums;

namespace ConversationService.Domain.Entities;

public class Conversation
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
    /// Дата обновления записи
    /// </summary>
    public DateTimeOffset UpdateDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Текст ответа
    /// </summary>
    public string Answer { get; set; }
}
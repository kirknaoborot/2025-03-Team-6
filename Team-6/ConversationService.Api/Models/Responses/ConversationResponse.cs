namespace ConversationService.Api.Mapping.Responses;

public class ConversationResponse
{
    /// <summary>
    /// Идентификатор обращения
    /// </summary>
    public Guid ConversationId { get; set; }
    
    /// <summary>
    /// Канал
    /// </summary>
    public string Channel { get; set; }
    
    /// <summary>
    /// Сообщение обращения
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Статус /*отдельный класс*/
    /// </summary>
    public string Status { get; set; }
    
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

    /// <summary>
    /// Номер обращения
    /// </summary>
    public string Number { get; set; }
}
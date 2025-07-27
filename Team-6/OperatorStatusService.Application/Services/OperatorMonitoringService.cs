using MassTransit;
using Microsoft.Extensions.Logging;
using OperatorStatusService.Domain.Interfaces;
using OperatorStatusService.Domain.Message;


public class OperatorMonitoringService
{
    private readonly IOperatorStatusService _authService;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ILogger<OperatorMonitoringService> _logger;

    public OperatorMonitoringService(
        IOperatorStatusService authService,
        ISendEndpointProvider sendEndpointProvider,
        ILogger<OperatorMonitoringService> logger)
    {
        _authService = authService;
        _sendEndpointProvider = sendEndpointProvider;
        _logger = logger;
    }

    public async Task MonitorOperatorsAsync(CancellationToken ct)
    {
        var operators = await _authService.GetActiveOperatorsAsync(ct);
        _logger.LogInformation("Найдено активных операторов: {Count}", operators.Count);

        foreach (var op in operators)
        {
            _logger.LogInformation("Отправка: Operator '{fullName}' онлайн", op.FullName);

            // Получаем endpoint для отправки в конкретную очередь
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri("queue:operator.status"));

            var message = new OperatorOnlineEvent
            {
                FullName = op.FullName,
                Timestamp = DateTime.UtcNow,
                Id = Guid.NewGuid()
            };

            await endpoint.Send(message, context =>
            {
                context.MessageId = Guid.NewGuid();
            }, ct);

            _logger.LogInformation("Сообщение отправлено: {fullName} с ID: {messageId}",
                op.FullName, message.Id);

            await Task.Delay(50, ct); 
        }
    }
}

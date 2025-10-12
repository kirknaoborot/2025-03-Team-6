using Infrastructure.Shared.Contracts;
using MassTransit;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace ConversationDistributed.Services;

public class ConvState
{
    private readonly IAgentStateService _agentStateService;
    private readonly IBus _bus;
    private readonly ILogger<ConvState> _logger;

    private readonly Channel<DefineOperatorForConversationCommand> _queue = Channel.CreateUnbounded<DefineOperatorForConversationCommand>();

    public ConvState(
        IAgentStateService agentStateService,
        IBus bus,
        ILogger<ConvState> logger)
    {
        _agentStateService = agentStateService;
        _bus = bus;
        _logger = logger;

        _ = Task.Run(ProcessQueueAsync);
    }

    /// <summary>
    /// Добавляет новое обращение
    /// </summary>
    public async Task AddConversationAsync(DefineOperatorForConversationCommand command)
    {
        await _queue.Writer.WriteAsync(command);
        _logger.LogInformation("Обращение {ConversationId} добавлено в очередь.", command.ConversationId);
    }

    /// <summary>
    /// Основной обработчик очереди
    /// </summary>
    private async Task ProcessQueueAsync()
    {
        await foreach (var command in _queue.Reader.ReadAllAsync())
        {
            try
            {
                bool assigned = await TryAssignOperatorAsync(command);

                if (!assigned)
                {
                    _logger.LogWarning("Нет свободных операторов для {ConversationId}, повторная попытка через 1с", command.ConversationId);
                    _ = Task.Delay(1000).ContinueWith(async _ =>
                    {
                        await _queue.Writer.WriteAsync(command);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке обращения {ConversationId}", command.ConversationId);
            }
        }
    }

    /// <summary>
    /// Попытка назначить свободного оператора.
    /// </summary>
    private async Task<bool> TryAssignOperatorAsync(DefineOperatorForConversationCommand command)
    {
        var freeOperator = _agentStateService.GetFirstFreeOperator();

        if (freeOperator != null)
        {
            await _bus.Publish(new DefineAgentEvent
            {
                ConversationId = command.ConversationId,
                WorkerId = freeOperator.Id,
                MessageText = command.MessageText,
                CreateDate = command.CreateDate,
                Channel = command.Channel,
                ChannelSettingsId = command.ChannelSettingsId,
                UserId = command.UserId
            });

            _agentStateService.AssignConversationToUser(freeOperator.Id, command.ConversationId);
            _logger.LogInformation("Обращение {ConversationId} назначено оператору {WorkerId}", command.ConversationId, freeOperator.Id);
            
            return true;
        }

        return false;
    }
}

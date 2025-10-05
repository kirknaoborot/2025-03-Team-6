using Infrastructure.Shared.Contracts;
using MassTransit;
using System.Collections.Concurrent;

namespace ConversationDistributed.Services;

    public class ConvState //: BackgroundService
    {
        private readonly IAgentStateService _agentStateService;
        private readonly IBus _bus;
        private readonly ILogger<ConvState> _logger;

        // Храним информацию о "висящих" обращениях
        private readonly ConcurrentDictionary<Guid, PendingConversation> _pendingConversations
            = new();

        public ConvState(
            IAgentStateService agentStateService,
            IBus bus,
            ILogger<ConvState> logger)
        {
            _agentStateService = agentStateService;
            _bus = bus;
            _logger = logger;
        }

        // Метод для добавления нового обращения в очередь ожидания
        public async Task AddConversationAsync(DefineOperatorForConversationCommand command)
        {
            var conversation = new PendingConversation(command);

            if (_pendingConversations.TryAdd(command.ConversationId, conversation))
            {
                _logger.LogInformation("Обращение {ConversationId} добавлено в очередь ожидания.", command.ConversationId);
            }
            else
            {
                _logger.LogWarning("Обращение {ConversationId} уже находится в очереди.", command.ConversationId);
            }
        }

        // Попытка назначить оператора на обращение
        public async Task<bool> TryAssignOperatorAsync(Guid conversationId)
        {
            if (!_pendingConversations.TryGetValue(conversationId, out var pending))
                return true; // уже обработано или удалено

            if (pending.IsTimedOut())
            {
				await _bus.Publish(new DefineAgentEvent
				{
					ConversationId = conversationId,
					WorkerId = Guid.Empty,
					MessageText = pending.Command.MessageText,
					CreateDate = pending.Command.CreateDate,
					Channel = pending.Command.Channel,
					ChannelSettingsId = pending.Command.ChannelSettingsId,
					UserId = pending.Command.UserId,
					Answer = "На данный момент нет свободного агента. Пожалуйста, обратитесь позже."
				});
				_logger.LogWarning("Обращение {ConversationId} отменено по таймауту.", conversationId);
                _pendingConversations.TryRemove(conversationId, out _);
                return true;
            }

            var freeOperator = _agentStateService.GetFirstFreeOperator();
            if (freeOperator != null)
            {
                // Назначаем оператора
                _agentStateService.AssignConversationToUser(freeOperator.Id, pending.Command.ConversationId);

                // Отправляем событие
                await _bus.Publish(new DefineAgentEvent
                {
                    ConversationId = pending.Command.ConversationId,
                    WorkerId = freeOperator.Id,
                    MessageText = pending.Command.MessageText,
                    CreateDate = pending.Command.CreateDate,
                    Channel = pending.Command.Channel
                });

                // Удаляем из очереди
                _pendingConversations.TryRemove(conversationId, out _);

                _logger.LogInformation("Обращение {ConversationId} назначено оператору {WorkerId}.", conversationId, freeOperator.Id);
                return true;
            }

            return false; // не назначено — продолжаем ждать
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
				_logger.LogInformation($"Количество необработанных обращений {_pendingConversations?.Count}.");
				var tasks = _pendingConversations.Keys
                        .Select(conversationId => TryAssignOperatorAsync(conversationId))
                        .ToArray();

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке очереди ожидания обращений.");
                }

                await Task.Delay(1000, stoppingToken); // раз в секунду
            }
        }

        // Внутренний класс для хранения данных о "ожидающем" обращении
        private class PendingConversation
        {
            public DefineOperatorForConversationCommand Command { get; }
            public DateTime EnqueueTime { get; }

            public PendingConversation(DefineOperatorForConversationCommand command)
            {
                Command = command;
                EnqueueTime = DateTime.UtcNow;
            }

            public bool IsTimedOut() => DateTime.UtcNow - EnqueueTime > TimeSpan.FromSeconds(10);
        }
    }

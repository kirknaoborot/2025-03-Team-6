using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using ConversationState.Services;
using Infrastructure.Shared.Contracts;
using MassTransit;

namespace ConversationState.Consumer
{
    public class DefineOperatorForConversationStateConsumer : IConsumer<DefineOperatorForConversationCommand>
    {
        private readonly IAgentStateService _agentStateService;
        private readonly ConvState _conversationStateService;
        private readonly ILogger<DefineOperatorForConversationConsumer> _logger;

        public DefineOperatorForConversationStateConsumer(
            IAgentStateService agentStateService,
            ConvState conversationStateService,
            ILogger<DefineOperatorForConversationConsumer> logger)
        {
            _agentStateService = agentStateService;
            _conversationStateService = conversationStateService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DefineOperatorForConversationCommand> context)
        {
            var command = context.Message;
            _logger.LogInformation("Получено обращение: {ConversationId}, Сообщение: {MessageText}", command.ConversationId, command.MessageText);

            // Сначала пытаемся назначить оператора сразу
            var freeOperator = _agentStateService.GetFirstFreeOperator();

            if (freeOperator != null)
            {
                _agentStateService.AssignConversationToUser(freeOperator.Id, command.ConversationId);

                await context.Publish(new DefineAgentEvent
                {
                    ConversationId = command.ConversationId,
                    WorkerId = freeOperator.Id,
                    MessageText = command.MessageText,
                    CreateDate = command.CreateDate,
                    Channel = command.Channel
                });

                _logger.LogInformation("Оператор {WorkerId} назначен на обращение {ConversationId}.", freeOperator.Id, command.ConversationId);
            }
            else
            {
                // Нет свободного оператора — ставим в очередь ожидания
                await _conversationStateService.AddConversationAsync(command);
                _logger.LogInformation("Нет свободных операторов. Обращение {ConversationId} поставлено в очередь ожидания.", command.ConversationId);
            }
        }
    }
}

using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace OnlineStatusService.Consumers
{
    public class NotifySendConsumer : IConsumer<NotifySendCommand>
    {
        private readonly IHubContext<OnlineStatusHub> _hubContext;
        private readonly ILogger<NotifySendConsumer> _logger;

        public NotifySendConsumer(IHubContext<OnlineStatusHub> hubContext, ILogger<NotifySendConsumer> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NotifySendCommand> context)
        {
            var msg = context.Message;
            _logger.LogInformation($"{nameof(NotifySendConsumer)}.{nameof(Consume)}() -> The request has been assigned to the operator {msg.AgentId}");

            await _hubContext.Clients.All.SendAsync("ConversationDistributed", $"Распределено обращение: {msg.AgentId}");
        }
    }
}

using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace OnlineStatusService.Consumers
{
    public class NotifySendConsumer : IConsumer<NotifySendCommand>
    {
        private readonly IHubContext<OnlineStatusHub> _hubContext;

        public NotifySendConsumer(IHubContext<OnlineStatusHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<NotifySendCommand> context)
        {
            var msg = context.Message;

            Console.WriteLine($"Обращение распределено на оператора {msg.AgentId}");

            await _hubContext.Clients.All.SendAsync("ConversationDistributed", $"Распределено обращение: {msg.AgentId}");
        }
    }
}

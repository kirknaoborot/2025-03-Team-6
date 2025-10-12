using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace OnlineStatusService.Consumers
{
    public class ConversationDistributedConsumer : IConsumer<DefineAgentEvent>
    {
        private readonly IHubContext<OnlineStatusHub> _hubContext;

        public ConversationDistributedConsumer(IHubContext<OnlineStatusHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<DefineAgentEvent> context)
        {
            var msg = context.Message;

            Console.WriteLine($"Обращение {msg.ConversationId} распределено на оператора {msg.WorkerId}");

            await _hubContext.Clients.All.SendAsync("ConversationDistributed", $"Распределено обращение: {msg.ConversationId}";
        }
    }
}

using MassTransit;
using MessageHubService.Domain.Entities;

namespace TestConsumer
{
    class EventConsumer : IConsumer<ClientMessage>
    {
        public async Task Consume(ConsumeContext<ClientMessage> context)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine($"message id {context.Message.Id}, message text {context.Message.MessageText}");
        }
    }
}

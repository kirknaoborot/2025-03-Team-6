using Infrastructure.Shared;
using MassTransit;

namespace OrchestratService.Application
{
    public class ClientMessageEventConsumer : IConsumer<ClientMessage>
    {
        public async Task Consume(ConsumeContext<ClientMessage> context)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine($"message id {context.Message.Id}, message text {context.Message.MessageText}");
        }
    }
}

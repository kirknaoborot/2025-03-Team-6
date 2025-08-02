using ConversationService.Infrastructure.Messaging.Events;
using MassTransit;

namespace ConversationService.Infrastructure.Messaging.Consumers;

public class ConversationCreatedConsumer : IConsumer<ConversationCreated>
{
    public Task Consume(ConsumeContext<ConversationCreated> context)
    {
        var message = context.Message;

        // Например: логирование, сохранение и т.д.
        Console.WriteLine($"[RabbitMQ] Получено обращение: {message.ConversationId}");

        return Task.CompletedTask;
    }
}
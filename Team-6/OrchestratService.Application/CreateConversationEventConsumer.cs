using Infrastructure.Shared.Contracts;
using MassTransit;

namespace OrchestratService.Application
{
	public class CreateConversationEventConsumer : IConsumer<CreateConversationEvent>
	{
		public async Task Consume(ConsumeContext<CreateConversationEvent> context)
		{
			Console.WriteLine($"CreateConversationEventConsumer => message = '{context.Message.Message}', create date = '{context.Message.CreateDate}', conversation id = '{context.Message.ConversationId}'");
			await Task.Delay(TimeSpan.FromSeconds(1));
		}
	}
}

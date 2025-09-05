using Infrastructure.Shared;

namespace MessageHubService.Application.Interfaces
{
    public interface IBotWork
    {
        Task Start();
        Task Stop();
		Task SentMessageToClient(SendMessageDto sendMessageDto);
    }
}

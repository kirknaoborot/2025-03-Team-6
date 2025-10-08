using Infrastructure.Shared;

namespace MessageHubService.Application.Interfaces
{
    public interface IBotWork
    {
        Task StartAsync();
        Task StopAsync();
		Task SentMessageToClientAsync(SendMessageDto sendMessageDto);
    }
}

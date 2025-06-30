namespace MessageHubService.Application.Interfaces
{
    public interface IBotWork
    {
        Task Start();
        Task Stop();
    }
}

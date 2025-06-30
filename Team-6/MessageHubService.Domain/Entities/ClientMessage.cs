namespace MessageHubService.Domain.Entities
{
    public record ClientMessage(int Id, string MessageText, DateTime SendData, string Priority)
    {
    }
}

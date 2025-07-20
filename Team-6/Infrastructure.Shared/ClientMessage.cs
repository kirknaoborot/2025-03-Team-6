namespace Infrastructure.Shared
{
    public record ClientMessage(int Id, string MessageText, DateTime SendData, string Priority)
    {
    }
}

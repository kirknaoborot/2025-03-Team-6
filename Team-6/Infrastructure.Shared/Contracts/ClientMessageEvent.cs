namespace Infrastructure.Shared.Contracts;

public class ClientMessageEvent
{
    public int Id { get; set; }
    public string MessageText { get; set; }
    public DateTime SendData { get; set; }
    public string Priority { get; set; }
}

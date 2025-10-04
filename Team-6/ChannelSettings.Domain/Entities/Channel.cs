
namespace ChannelSettings.Domain.Entities
{
    public class Channel : Interfaces.IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}

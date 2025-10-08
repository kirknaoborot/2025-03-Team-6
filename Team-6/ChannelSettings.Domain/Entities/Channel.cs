using Infrastructure.Shared.Enums;

namespace ChannelSettings.Domain.Entities
{
    public class Channel : Interfaces.IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Token { get; set; } 
        public ChannelType Type { get; set; }
    }
}

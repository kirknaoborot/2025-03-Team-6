using Infrastructure.Shared.Enums;

namespace ChannelSettings.Core.Models
{
    public class UpdatingChannel
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public ChannelType Type { get; set; }
    }
}

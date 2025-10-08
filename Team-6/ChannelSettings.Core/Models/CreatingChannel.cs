using Infrastructure.Shared.Enums;

namespace ChannelSettings.Core.Core
{
    public class CreatingChannel
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public ChannelType Type { get; set; }
    }
}

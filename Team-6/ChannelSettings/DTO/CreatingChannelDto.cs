using Infrastructure.Shared.Enums;

namespace ChannelSettings.DTO
{
    public class CreatingChannelDto
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public ChannelType Type { get; set; }
    }
}

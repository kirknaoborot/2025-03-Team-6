using Infrastructure.Shared.Enums;

namespace ChannelSettings.DTO
{
    public class ChannelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public ChannelType Type { get; set; }
    }
}

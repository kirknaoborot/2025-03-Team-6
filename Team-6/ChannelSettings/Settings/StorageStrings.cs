using ChannelSettings.Core.IServices;

namespace ChannelSettings.Settings
{
    public class StorageStrings : IStorageStrings
    {
        public string ApplicationDbContext { get; set; } = string.Empty;
    }
}

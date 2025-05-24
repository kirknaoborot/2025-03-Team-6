using CitizenRequests.Core.IRepositories;

namespace CitizenRequests.API.Settings
{
    public class StorageStrings : IStorageStrings
    {
        public string DBConnection { get; set; } = string.Empty;
        public string FileDB { get; set; } = string.Empty;
    }
}


using System.Text.Json.Serialization;

namespace OperatorStatusService.Domain.Models
{
    public class UserInfo
    {
        [JsonPropertyName("FullName")]
        public string FullName { get; set; }

        [JsonPropertyName("isActive")]
        public string IsActiveRaw { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("lastSeen")]
        public string LastSeen { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonIgnore]
        public bool IsActive => !string.IsNullOrEmpty(IsActiveRaw);
    }
}

using Microsoft.AspNetCore.Identity;

namespace CitizenRequest.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Full_name { get; set; }
        public string Login { get; set; }
        public string Password_hash { get; set; }
        public string Is_active { get; set; }
        public string Status { get; set; }
        public DateTime? Last_seen { get; set; }
        public string Role { get; set; }
    }
}

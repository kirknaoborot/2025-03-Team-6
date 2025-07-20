using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Login { get; set; }
        public string PasswordsHash { get; set; }
        public string IsActive { get; set; }
        public string Status { get; set; }
        public DateTime? LastSeen { get; set; }
        public string Role { get; set; }
    }
}

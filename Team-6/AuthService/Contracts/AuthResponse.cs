namespace AuthService.Contracts
{
    public class AuthResponse 
    {
        public string FullName { get; set; }
        public string IsActive { get; set; }
        public string Status { get; set; } 
        public DateTime? LastSeen { get; set; } 
        public string Role { get; set; } 
        public string Login { get; set; }
    }
}

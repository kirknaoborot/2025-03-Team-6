
namespace Authorization.Models
{
    public class User
    {
        public const int max_name_lenght = 250;
        public User(string fullName, string login, string PasswordHash, string isActive, string status, DateTime? lastSeen, string role)
        {
            FullName= fullName;
            Login = login;
            PasswordsHash  = PasswordHash;
            IsActive = isActive;
            Status = status;
            LastSeen = lastSeen;
            Role = role;
        }

        public string FullName { get; set; }
        public string Login { get; set; }
        public string PasswordsHash { get; set; }
        public string IsActive { get; set; }
        public string Status { get; set; }
        public DateTime? LastSeen { get; set; }
        public string Role { get; set; }

        public static (User user, string error) Create(string FullName, string Login, string PasswordsHash, string IsActive, string Status, DateTime? LastSeen, string Role)
        {
            var error = string.Empty;
            if (string.IsNullOrEmpty(FullName) || FullName.Length > max_name_lenght)
            {
                error = "ФИО не может быть пустым или содержать более 250 символов";
            }
            var user = new User( FullName,  Login,  PasswordsHash,  IsActive,  Status,  LastSeen,  Role);
            return (user, error); 
        }
    }
}

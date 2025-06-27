
namespace Authorization.Models
{
    public class User
    {
        public const int max_name_lenght = 250;
        public User(string full_name, string login, string password_hash, string is_active, string status, DateTime? last_seen, string role)
        {
            Full_name= full_name;
            Login = login;
            Password_hash = password_hash;
            Is_active = is_active;
            Status = status;
            Last_seen = last_seen;
            Role = role;
        }

        public string Full_name { get; set; }
        public string Login { get; set; }
        public string Password_hash { get; set; }
        public string Is_active { get; set; }
        public string Status { get; set; }
        public DateTime? Last_seen { get; set; }
        public string Role { get; set; }

        public static (User user, string error) Create(string full_name, string login, string password_hash, string is_active, string status, DateTime? last_seen, string role)
        {
            var error = string.Empty;
            if (string.IsNullOrEmpty(full_name) || full_name.Length > max_name_lenght)
            {
                error = "ФИО не может быть пустым или содержать более 250 символов";
            }
            var user = new User( full_name,  login,  password_hash,  is_active,  status,  last_seen,  role);
            return (user, error); 
        }
    }
}

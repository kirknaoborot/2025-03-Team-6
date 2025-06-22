namespace Auth.Core.Dto
{
    public class UserDto
    {

        public string ID { get; set; }
        public string Full_name { get; set; }
        public string Login { get; set; }
        public string Is_active { get; set; }
        public string Status { get; set; }
        public DateTime? Last_seen { get; set; }
        public string Role { get; set; }
    }
}

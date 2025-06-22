namespace AuthService.Settings
{
    public class ConnectionOptions
    {
        public const string Section = "ConnectionStrings";
        public string ApplicationDbContext { get; set; }
        public RmqSettings RmqSettings { get; set; }
    }
}

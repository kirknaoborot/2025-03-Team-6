namespace ChannelSettings.Settings
{
    public class RmqSettings
    {
        public const string Section = "RmqSettings";
        public string Login { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string VHost { get; set; }
    }
}

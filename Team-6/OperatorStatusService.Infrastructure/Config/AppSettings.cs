namespace OperatorStatusService.Infrastructure.Config
{
    public class AppSettings
    {
        public AuthApiConfig AuthApi { get; set; }
        public RabbitMqConfig RabbitMq { get; set; }
    }

    public class AuthApiConfig
    {
        public string Url { get; set; }
    }

    public class RabbitMqConfig
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; } 
    }
}
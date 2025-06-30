namespace MessageHubService.Domain.Entities
{
    public class TelegramBotOptionsList
    {
        public List<TelegramBotOptions> TelegramBotListOptions { get; set; }
    }

    public class TelegramBotOptions
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }
}

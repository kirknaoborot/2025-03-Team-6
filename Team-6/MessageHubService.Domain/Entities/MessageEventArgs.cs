namespace MessageHubService.Domain.Entities
{
    public sealed class MessageEventArgs : EventArgs
    {
        public int Id { get; set; }

		public long UserId { get; set; }

        public string? Text { get; set; }

        public DateTime SendData { get; set; }

		public int ChannelSettingId { get; set; }
    }
}

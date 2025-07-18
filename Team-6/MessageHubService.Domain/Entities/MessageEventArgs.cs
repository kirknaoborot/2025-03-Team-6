﻿namespace MessageHubService.Domain.Entities
{
    public sealed class MessageEventArgs : EventArgs
    {
        public int Id { get; set; }

        public string? Text { get; set; }

        public DateTime SendData { get; set; }
    }
}

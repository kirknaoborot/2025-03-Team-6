namespace Infrastructure.Shared
{
    public class RabbitMqOptions
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

		/// <summary>
		/// Название очереди для события отправки сообщения с стороны клиента
		/// </summary>
		public string ClientMessageEventQueue { get; set; }

		/// <summary>
		/// Название очереди для события создания обращения
		/// </summary>
		public string ConversationEventQueue { get; set; }

		public string DefineAgentEvent { get; set; }

	}
}

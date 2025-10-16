using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Application.Interfaces;
using MessageHubService.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageHubService.Application.Services.TelegramBot
{
	public static class BusProvider
	{
		public static IBus Bus { get; private set; }
		public static void Initialize(IBus bus) => Bus = bus;
	}

	public class TelegramBotService : IBotWork, IMessageEvent
    {
        private TelegramBotClient _bot;
        private Task<User> _me;
        private CancellationTokenSource _cancellationToken;
        private TelegramMessageHandler _handler;

        public event EventHandler<MessageEventArgs>? OnIncomingMessage;
        public event EventHandler<MessageEventArgs>? OnOutgoingMessage;

        public int ChannelId { get; set; }
        public string Name { get; private set; }
        public string Token { get; private set; }

        public TelegramBotService()
        {
           
        }

		private async Task HandleIncomingMessageAsync(MessageEventArgs e)
		{
			try
			{
				var newInMessage = new ClientMessageEvent
				{
					Id = e.Id,
					UserId = e.UserId,
					MessageText = e.Text,
					SendData = e.SendData,
					Priority = "high",
					Channel = Infrastructure.Shared.Enums.ChannelType.Telegram,
					ChannelSettingsId = e.ChannelSettingId,
				};

				await BusProvider.Bus.Publish(newInMessage);
			}
			catch (OperationCanceledException)
			{
				// ignore on shutdown
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to publish incoming message {MessageId}", e.Id);
			}
		}

		public void SetTokenAndName(string token, string name)
		{
			Name = name;
			Token = token;
		}

		public void DefineTelegramBotClient()
		{
			_cancellationToken = new CancellationTokenSource();
			_bot = new TelegramBotClient(Token, cancellationToken: _cancellationToken.Token);
			_me = _bot.GetMe();

			OnIncomingMessage += (sender, args) =>
			{
				_ = HandleIncomingMessageAsync(args);
			};

			_handler = new TelegramMessageHandler(_bot, _me, () => ChannelId, message =>
			{
				OnIncomingMessage?.Invoke(this, message);
			});
		}

        public async Task StartAsync()
        {
            _bot.OnError += _handler.OnErrorAsync;
            _bot.OnMessage += _handler.OnMessageAsync;
            _bot.OnUpdate += _handler.OnUpdateAsync;

            await _bot.GetMe();
        }

        public async Task StopAsync()
        {
            _bot.OnError -= _handler.OnErrorAsync;
            _bot.OnMessage -= _handler.OnMessageAsync;
            _bot.OnUpdate -= _handler.OnUpdateAsync;

            await _cancellationToken.CancelAsync();
        }

		public async Task SentMessageToClientAsync(SendMessageDto sendMessageDto)
		{
			Console.WriteLine($"===> SentMessageToClient => sendMessageDto.UserId = '{sendMessageDto.UserId}', sendMessageDto.MessageText = '{sendMessageDto.MessageText}'");

			await _bot.SendMessage(sendMessageDto.UserId, sendMessageDto.MessageText);
		}
	}
}

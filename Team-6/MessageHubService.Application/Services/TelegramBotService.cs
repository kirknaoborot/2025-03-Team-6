using Infrastructure.Shared;
using MessageHubService.Application.Interfaces;
using MessageHubService.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageHubService.Application.Services
{
    public class TelegramBotService : IBotWork, IMessageEvent
    {
        private TelegramBotClient _bot;
        private Task<User> _me;
        private CancellationTokenSource _cancellationToken;

        public event EventHandler<MessageEventArgs>? OnIncomingMessage;
        public event EventHandler<MessageEventArgs>? OnOutgoingMessage;

        public int ChannelId { get; set; }
        public string Name { get; private set; }
        public string Token { get; private set; }

        public TelegramBotService()
        {
           
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
		}

        public async Task StartAsync()
        {
            _bot.OnError += OnErrorAsync;
            _bot.OnMessage += OnMessageAsync;
            _bot.OnUpdate += OnUpdateAsync;

            await _bot.GetMe();
        }

        public async Task StopAsync()
        {
            _bot.OnError -= OnErrorAsync;
            _bot.OnMessage -= OnMessageAsync;
            _bot.OnUpdate -= OnUpdateAsync;

            await _cancellationToken.CancelAsync();
        }

        async Task OnErrorAsync(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
        }

        async Task OnMessageAsync(Message msg, UpdateType type)
        {
            if (msg.Text is not { } text)
            {
                Console.WriteLine($"Received a message of type {msg.Type}");
            }  
            else if (text.StartsWith('/'))
            {
                var space = text.IndexOf(' ');
                if (space < 0) space = text.Length;
                var command = text[..space].ToLower();
                if (command.LastIndexOf('@') is > 0 and int at) // it's a targeted command
                    if (command[(at + 1)..].Equals(_me.Result.Username, StringComparison.OrdinalIgnoreCase))
                        command = command[..at];
                    else
                        return; // command was not targeted at me
            }
            else
                await OnTextMessageAsync(msg);
        }

        async Task OnTextMessageAsync(Message msg) // received a text message that is not a command
        {
            Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");

            var message = new MessageEventArgs
            {
                Id = msg.Id,
				UserId = msg.Chat.Id,
                Text = msg.Text,
                SendData = DateTime.UtcNow,
                ChannelSettingId = ChannelId
            };

            await Task.Run(() =>
            {
                OnIncomingMessage?.Invoke(this, message);
            });
        }

        async Task OnUpdateAsync(Update update)
        {
            switch (update)
            {
                case { CallbackQuery: { } callbackQuery }: await OnCallbackQueryAsync(callbackQuery); break;
                case { PollAnswer: { } pollAnswer }: await OnPollAnswerAsync(pollAnswer); break;
                default: Console.WriteLine($"Received unhandled update {update.Type}"); break;
            };
        }

        async Task OnCallbackQueryAsync(CallbackQuery callbackQuery)
        {
            await _bot.AnswerCallbackQuery(callbackQuery.Id, $"You selected {callbackQuery.Data}");
            await _bot.SendMessage(callbackQuery.Message!.Chat, $"Received callback from inline button {callbackQuery.Data}");
        }

        async Task OnPollAnswerAsync(PollAnswer pollAnswer)
        {
            if (pollAnswer.User != null)
                await _bot.SendMessage(pollAnswer.User.Id, $"You voted for option(s) id [{string.Join(',', pollAnswer.OptionIds)}]");
        }

		public async Task SentMessageToClientAsync(SendMessageDto sendMessageDto)
		{
			Console.WriteLine($"===> SentMessageToClient => sendMessageDto.UserId = '{sendMessageDto.UserId}', sendMessageDto.MessageText = '{sendMessageDto.MessageText}'");

			await _bot.SendMessage(sendMessageDto.UserId, sendMessageDto.MessageText);
		}
	}
}

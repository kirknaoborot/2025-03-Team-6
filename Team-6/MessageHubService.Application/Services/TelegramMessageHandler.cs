using MessageHubService.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageHubService.Application.Services
{
    public class TelegramMessageHandler
    {
        private readonly TelegramBotClient _bot;
        private readonly Task<Telegram.Bot.Types.User> _me;
        private readonly Func<int> _getChannelId;
        private readonly Action<MessageEventArgs> _onIncomingMessage;

        public TelegramMessageHandler(
            TelegramBotClient bot,
            Task<Telegram.Bot.Types.User> me,
            Func<int> getChannelId,
            Action<MessageEventArgs> onIncomingMessage)
        {
            _bot = bot;
            _me = me;
            _getChannelId = getChannelId;
            _onIncomingMessage = onIncomingMessage;
        }

        public async Task OnErrorAsync(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
        }

        public async Task OnMessageAsync(Message msg, UpdateType type)
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
                if (command.LastIndexOf('@') is > 0 and int at)
                    if (command[(at + 1)..].Equals(_me.Result.Username, StringComparison.OrdinalIgnoreCase))
                        command = command[..at];
                    else
                        return;
            }
            else
                await OnTextMessageAsync(msg);
        }

        public async Task OnTextMessageAsync(Message msg)
        {
            Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");

            var message = new MessageEventArgs
            {
                Id = msg.Id,
                UserId = msg.Chat.Id,
                Text = msg.Text,
                SendData = DateTime.UtcNow,
                ChannelSettingId = _getChannelId()
            };

            await Task.Run(() =>
            {
                _onIncomingMessage(message);
            });
        }

        public async Task OnUpdateAsync(Update update)
        {
            switch (update)
            {
                case { CallbackQuery: { } callbackQuery }: await OnCallbackQueryAsync(callbackQuery); break;
                case { PollAnswer: { } pollAnswer }: await OnPollAnswerAsync(pollAnswer); break;
                default: Console.WriteLine($"Received unhandled update {update.Type}"); break;
            };
        }

        public async Task OnCallbackQueryAsync(CallbackQuery callbackQuery)
        {
            await _bot.AnswerCallbackQuery(callbackQuery.Id, $"You selected {callbackQuery.Data}");
            await _bot.SendMessage(callbackQuery.Message!.Chat, $"Received callback from inline button {callbackQuery.Data}");
        }

        public async Task OnPollAnswerAsync(PollAnswer pollAnswer)
        {
            if (pollAnswer.User != null)
                await _bot.SendMessage(pollAnswer.User.Id, $"You voted for option(s) id [{string.Join(',', pollAnswer.OptionIds)}]");
        }
    }
}


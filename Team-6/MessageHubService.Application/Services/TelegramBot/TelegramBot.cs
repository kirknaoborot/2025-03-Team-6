using Infrastructure.Shared;
using Infrastructure.Shared.Contracts;
using MassTransit;
using MessageHubService.Application.Interfaces;
using MessageHubService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageHubService.Application.Services.TelegramBot;

public class TelegramBot : IMessageEvent, IBot
{
	private readonly IBus _bus;
	private readonly ILogger<TelegramBot> _logger;
    private readonly ILogger<TelegramMessageHandler> _handlerLogger;

    private int _channelId;
	private TelegramBotClient _bot;
	private Task<User> _me;
	private CancellationTokenSource _cancellationToken;
	private TelegramMessageHandler _handler;

	public event EventHandler<MessageEventArgs>? OnIncomingMessage;
	public event EventHandler<MessageEventArgs>? OnOutgoingMessage;

	public TelegramBot(IBus bus, ILogger<TelegramBot> logger, ILogger<TelegramMessageHandler> handlerLogger)
	{
		_bus = bus;
		_logger = logger;
        _handlerLogger = handlerLogger;
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

			await _bus.Publish(newInMessage);
		}
		catch (OperationCanceledException)
		{
			// ignore on shutdown
		}
		catch (Exception ex)
		{
            _logger.LogError($"{nameof(TelegramBot)}.{nameof(HandleIncomingMessageAsync)}() -> Failed to publish incoming message {e.Id}");
		}
	}

	public async Task StartAsync()
	{
		_bot.OnError += _handler.OnErrorAsync;
		_bot.OnMessage += _handler.OnMessageAsync;
		_bot.OnUpdate += _handler.OnUpdateAsync;
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
		_logger.LogInformation($"{nameof(TelegramBot)}.{nameof(SentMessageToClientAsync)}() -> user id '{sendMessageDto.UserId}', message text '{sendMessageDto.MessageText}'");
		await _bot.SendMessage(sendMessageDto.UserId, sendMessageDto.MessageText);
	}

	public async Task CreateBotAsync(ChannelEvent channelEvent)
	{
		_channelId = channelEvent.Id;
		_cancellationToken = new CancellationTokenSource();
		_bot = new TelegramBotClient(channelEvent.Token, cancellationToken: _cancellationToken.Token);
		_me = _bot.GetMe();

		OnIncomingMessage += (sender, args) =>
		{
			_ = HandleIncomingMessageAsync(args);
		};

		_handler = new TelegramMessageHandler(_bot, _me, () => _channelId, _handlerLogger, message =>
		{
			OnIncomingMessage?.Invoke(this, message);
		});
	}

	public override int GetHashCode()
	{
		return _channelId;
	}

	public void Dispose()
	{
		_channelId = 0;
		_bot = null;
		_me = null;
		_cancellationToken = null;
		_handler = null;
	}
}

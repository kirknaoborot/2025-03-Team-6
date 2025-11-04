using Infrastructure.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using OnlineStatusService;
using System.Collections.Concurrent;

public class OnlineStatusHub : Hub
{
    private readonly IAgentStatusNotifier _notifier;
    private static readonly ConcurrentDictionary<string, Guid> Connections = new();
    private readonly ILogger<OnlineStatusHub> _logger;

    public OnlineStatusHub(IAgentStatusNotifier notifier, ILogger<OnlineStatusHub> logger)
    {
        _notifier = notifier;
        _logger = logger;
    }

    public async Task UserOnline(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogError($"ConnectionID: {Context.ConnectionId}; UserId не найден");
            return;
        }

        var obj = JObject.Parse(userId);
        var id = obj["id"]?.Value<string>();
        var agentId = Guid.TryParse(id, out var g) ? g : Guid.Empty;

        Connections[Context.ConnectionId] = agentId;
        await _notifier.PublishStatusAsync(agentId, AgentStatusType.Connect);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Connections.TryRemove(Context.ConnectionId, out var agentId))
        {
            await _notifier.PublishStatusAsync(agentId, AgentStatusType.Disconnect);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
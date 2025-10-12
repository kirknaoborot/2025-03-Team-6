using Infrastructure.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using OnlineStatusService;
using System.Collections.Concurrent;

public class OnlineStatusHub : Hub
{
    private readonly IAgentStatusNotifier _notifier;
    private static readonly ConcurrentDictionary<string, Guid> Connections = new();

    public OnlineStatusHub(IAgentStatusNotifier notifier)
    {
        _notifier = notifier;
    }

    public async Task UserOnline(string userId)
    {
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
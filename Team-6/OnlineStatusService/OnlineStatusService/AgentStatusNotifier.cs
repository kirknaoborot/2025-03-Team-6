using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using MassTransit;
using OnlineStatusService;

public class AgentStatusNotifier : IAgentStatusNotifier
{
    private readonly IBus _bus;
    private readonly ILogger<AgentStatusNotifier> _logger;

    public AgentStatusNotifier(IBus bus, ILogger<AgentStatusNotifier> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task PublishStatusAsync(Guid agentId, AgentStatusType status)
    {
        var evt = new AgentStatusEvent
        {
            AgentId = agentId,
            Date = DateTime.UtcNow,
            Status = status
        };
        _logger.LogInformation($"{nameof(AgentStatusNotifier)}.{nameof(PublishStatusAsync)}() -> [SignalR] Publish {status} for {agentId}");
        await _bus.Publish(evt);
    }
}
using Infrastructure.Shared.Enums;

namespace OnlineStatusService
{
    public interface IAgentStatusNotifier
    {
        Task PublishStatusAsync(Guid agentId, AgentStatusType status);
    }
}

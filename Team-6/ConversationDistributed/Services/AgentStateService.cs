using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;

namespace ConversationDistributed.Services
{
    public class AgentStateService : IAgentStateService
    {
        private List<Agent> agents = new();
        private readonly ILogger<AgentStateService> _logger;
        public AgentStateService(ILogger<AgentStateService> logger)
        {
            _logger = logger;
        }

        public void UserUpdateState(AgentStatusCommand agentInfo)
        {

            if (agentInfo.Status == AgentStatusType.Connect)
            {
                agents.Add(new Agent
                {
                    Id = agentInfo.AgentId,
                    IsFreeForConversation = true
                });
                _logger.LogInformation($"{nameof(AgentStateService)}.{nameof(UserUpdateState)}() -> Added agent: {agentInfo.AgentId}");
            }
            else if (agentInfo.Status == AgentStatusType.Disconnect)
            {
                var agent = agents.FirstOrDefault(x => x.Id == agentInfo.AgentId);

                if (agent is not null)
				{
					ReleaseConversationFromUser(agent.Id);
					agents.Remove(agent);
				}
                _logger.LogInformation($"{nameof(AgentStateService)}.{nameof(UserUpdateState)}() -> Agent disabled: {agentInfo.AgentId}");
            }
            _logger.LogInformation($"{nameof(AgentStateService)}.{nameof(UserUpdateState)}() -> Number of active agents: {agents.Count}");
        }

        public void AssignConversationToUser(Guid userId, Guid conversationId)
        {
            var agent = agents.FirstOrDefault(x => x.Id == userId);

            if (agent is not null)
            {
                agent.ActiveConverstionId = conversationId;
                agent.IsFreeForConversation = false;
            }
        }

        public void ReleaseConversationFromUser(Guid userId)
        {
            var agent = agents.FirstOrDefault(x => x.Id == userId);

            if (agent is not null)
            {
                agent.IsFreeForConversation = true;
            }
        }

		public Agent GetFirstFreeOperator()
        {
            _logger.LogInformation($"{nameof(AgentStateService)}.{nameof(GetFirstFreeOperator)}() -> Number of free operators: {agents.Count}");
            return agents.FirstOrDefault(x => x.IsFreeForConversation);
		}
	}
}

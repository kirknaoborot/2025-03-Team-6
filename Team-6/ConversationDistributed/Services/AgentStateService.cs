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

        public void UserUpdateState(AgentStatusEvent agentInfo)
        {

            if (agentInfo.Status == AgentStatusType.Connect)
            {
                agents.Add(new Agent
                {
                    Id = agentInfo.AgentId,
                    IsFreeForConversation = true
                });

                _logger.LogInformation("Добавлен агент: {@AgentId}", agentInfo.AgentId);
            }
            else if (agentInfo.Status == AgentStatusType.Disconnect)
            {
                var agent = agents.FirstOrDefault(x => x.Id == agentInfo.AgentId);

                if (agent is not null)
				{
					ReleaseConversationFromUser(agent.Id);
					agents.Remove(agent);
				}

                _logger.LogInformation("Отключен агент: {@AgentId}", agentInfo.AgentId);
            }

            _logger.LogInformation("Количество активных агентов: {AgentCount}", agents.Count);
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
            _logger.LogInformation($"agents GetFirstFreeOperator: ========> {agents.Count}");
            return agents.FirstOrDefault(x => x.IsFreeForConversation);
		}
	}
}

using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using System.Collections.Concurrent;

namespace ConversationDistributed.Services
{
    public class AgentStateService : IAgentStateService
    {
        private List<Agent> agents = new();

        public void UserUpdateState(AgentStatusEvent agentInfo)
        {

            if (agentInfo.Status == AgentStatusType.Connect)
            {
                agents.Add(new Agent
                {
                    Id = agentInfo.AgentId,
                    IsFreeForConversation = true
                });

                Console.WriteLine($"Добавлен агент: ========> {agentInfo.AgentId}");
            }
            else if (agentInfo.Status == AgentStatusType.Disconnect)
            {
                var agent = agents.FirstOrDefault(x => x.Id == agentInfo.AgentId);

                if (agent is not null)
                {
                    agents.Remove(agent);
                }

                Console.WriteLine($"Отключен агент: ========> {agentInfo.AgentId}");
            }

            Console.WriteLine($"Кол-во активных агентов: ========> {agents.Count}");
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
            Console.WriteLine($"agents GetFirstFreeOperator: ========> {agents.Count}");
            return agents.FirstOrDefault(x => x.IsFreeForConversation);
		}
	}
}

using Infrastructure.Shared.Contracts;
using System.Collections.Concurrent;

namespace ConversationDistributed.Services
{
    public class AgentStateService : IAgentStateService
    {
        private readonly ConcurrentDictionary<Guid, Agent> _agents
           = new();

        private List<Agent> agentsTest = [];

        public void UserLoggedIn(AgentStatusEvent agentInfo)
        {
			var agent = new Agent
			{
				Id = agentInfo.AgentId,
				IsFreeForConversation = true
			};

            Console.WriteLine($"agents: ========> {agentsTest.Count}");
            agentsTest.Add(agent);
            _agents[agent.Id] = agent;
        }

        public void UserLoggedOut(Guid userId)
        {
            _agents.TryRemove(userId, out _);
        }

        public void AssignConversationToUser(Guid userId, Guid conversationId)
        {
            if (_agents.TryGetValue(userId, out var user))
            {
				user.ActiveConverstionId = conversationId;
				user.IsFreeForConversation = false;
            }
        }

        public void ReleaseConversationFromUser(Guid userId)
        {
            if (_agents.TryGetValue(userId, out var user))
            {
				user.IsFreeForConversation = true;
            }
        }

        public IEnumerable<Agent> GetAllOperators()
        {
			return _agents.Values;
        }

		public IEnumerable<Agent> GetAllFreeOperators()
		{
			return _agents.Values.Where(x => x.IsFreeForConversation);
		}

		public Agent GetFirstFreeOperator()
		{
            Console.WriteLine($"agents GetFirstFreeOperator: ========> {agentsTest.Count}");
            return agentsTest.FirstOrDefault(x => x.IsFreeForConversation);
		}
	}
}

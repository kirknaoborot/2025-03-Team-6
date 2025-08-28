using Infrastructure.Shared.Contracts;
using System.Collections.Concurrent;

namespace ConversationDistributed.Services
{
    public class AgentStateService : IAgentStateService
    {
        private readonly ConcurrentDictionary<Guid, Agent> _agents
           = new();

        public void UserLoggedIn(UserLoggedInEvent agentInfo)
        {
			var agent = new Agent
			{
				Id = agentInfo.Id,
				Login = agentInfo.Login,
				FullName = agentInfo.FullName,
				IsFreeForConversation = true
			};

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

        public string GetFullName(Guid userId)
        {
            return _agents.TryGetValue(userId, out var user) ? user.FullName : null;
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
			return _agents.Values.FirstOrDefault(x => x.IsFreeForConversation);
		}
	}
}

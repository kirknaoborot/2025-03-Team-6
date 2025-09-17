
using Infrastructure.Shared.Contracts;

namespace ConversationDistributed.Services
{
    public interface IAgentStateService
    {
        void AssignConversationToUser(Guid userId, Guid conversationId);
        IEnumerable<Agent> GetAllOperators();
		IEnumerable<Agent> GetAllFreeOperators();
		Agent GetFirstFreeOperator();
        void ReleaseConversationFromUser(Guid userId);
        void UserLoggedIn(AgentStatusEvent userInfo);
        void UserLoggedOut(Guid userId);
    }
}
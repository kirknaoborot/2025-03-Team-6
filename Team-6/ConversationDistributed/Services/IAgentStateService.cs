
using Infrastructure.Shared.Contracts;

namespace ConversationDistributed.Services
{
    public interface IAgentStateService
    {
        void AssignConversationToUser(Guid userId, Guid conversationId);
        IEnumerable<Agent> GetAllOperators();
		IEnumerable<Agent> GetAllFreeOperators();
		Agent GetFirstFreeOperator();
		string GetFullName(Guid userId);
        void ReleaseConversationFromUser(Guid userId);
        void UserLoggedIn(UserLoggedInEvent userInfo);
        void UserLoggedOut(Guid userId);
    }
}
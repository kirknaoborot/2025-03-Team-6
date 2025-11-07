
using Infrastructure.Shared.Contracts;

namespace ConversationDistributed.Services
{
    public interface IAgentStateService
    {
        void AssignConversationToUser(Guid userId, Guid conversationId);
		Agent GetFirstFreeOperator();
        void ReleaseConversationFromUser(Guid userId);
        void UserUpdateState(AgentStatusCommand userInfo);
    }
}
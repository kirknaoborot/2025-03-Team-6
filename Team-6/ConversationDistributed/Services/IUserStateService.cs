
namespace ConversationDistributed.Services
{
    public interface IUserStateService
    {
        void AssignConversationToUser(string login, Guid conversationId);
        IEnumerable<KeyValuePair<string, (string FullName, Guid? ActiveConversationId)>> GetAllUsers();
        string GetFullName(string login);
        bool HasActiveConversation(string login);
        void ReleaseConversationFromUser(string login);
        void UserLoggedIn(string login, string fullName);
        void UserLoggedOut(string login);
    }
}
using System.Collections.Concurrent;

namespace ConversationDistributed.Services
{
    public class UserStateService : IUserStateService
    {
        private readonly ConcurrentDictionary<string, (string FullName, Guid? ActiveConversationId)> _users
           = new();

        public void UserLoggedIn(string login, string fullName)
        {
            _users[login] = (fullName, null);
        }

        public void UserLoggedOut(string login)
        {
            _users.TryRemove(login, out _);
        }

        public void AssignConversationToUser(string login, Guid conversationId)
        {
            if (_users.TryGetValue(login, out var user))
            {
                _users[login] = (user.FullName, conversationId);
            }
        }

        public void ReleaseConversationFromUser(string login)
        {
            if (_users.TryGetValue(login, out var user))
            {
                _users[login] = (user.FullName, null);
            }
        }

        public bool HasActiveConversation(string login)
        {
            return _users.TryGetValue(login, out var user) && user.ActiveConversationId.HasValue;
        }

        public string GetFullName(string login)
        {
            return _users.TryGetValue(login, out var user) ? user.FullName : login;
        }

        public IEnumerable<KeyValuePair<string, (string FullName, Guid? ActiveConversationId)>> GetAllUsers()
        {
            return _users.ToArray(); // или .ToList()
        }
    }
}

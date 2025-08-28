using Infrastructure.Shared.Enums;

namespace Infrastructure.Shared.Contracts
{
    public record UserLoggedInEvent
    {
		public Guid Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public RoleType Role { get; set; }
        public DateTime LoginTime { get; set; }
    }
}

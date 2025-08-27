

namespace ConversationDistributed.Contracts
{
    public record UserAssignmentProposal
    {
        public string FullName { get; init; }
        public Guid? ConversationId { get; init; }
        public bool CanTake { get; init; }
        public string Reason { get; init; }
    }
}

namespace ConversationDistributed;

public class Agent()
{
	public Guid Id { get; set; }
	public Guid ActiveConverstionId { get; set; }

	public bool IsFreeForConversation { get; set; }
}

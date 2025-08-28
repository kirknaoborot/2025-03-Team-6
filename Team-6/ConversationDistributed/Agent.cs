namespace ConversationDistributed;

public class Agent()
{
	public Guid Id { get; set; }
	public Guid ActiveConverstionId { get; set; }
	public string Login { get; set; }
	public string FullName { get; set; }
	public bool IsFreeForConversation { get; set; }
}

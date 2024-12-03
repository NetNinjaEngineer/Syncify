namespace Syncify.Domain.Entities;

public class GroupConversation : Conversation
{
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
}
namespace Syncify.Domain.Entities;
public class Conversation : BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? LastMessageAt { get; set; }
    public ICollection<Message> Messages { get; set; } = [];
}

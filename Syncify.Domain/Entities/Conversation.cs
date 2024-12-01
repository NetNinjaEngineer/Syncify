using Syncify.Domain.Entities.Identity;

namespace Syncify.Domain.Entities;
public sealed class Conversation : BaseEntity
{
    public string SenderUserId { get; set; } = null!;
    public ApplicationUser SenderUser { get; set; } = null!;
    public string ReceiverUserId { get; set; } = null!;
    public ApplicationUser ReceiverUser { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? LastMessageAt { get; set; }
    public ICollection<Message> Messages { get; set; } = [];
}

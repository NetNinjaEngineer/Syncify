using Syncify.Domain.Enums;

namespace Syncify.Domain.Entities;

public sealed class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public string? Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? UpdatedAt { get; set; }
    public MessageStatus MessageStatus { get; set; }
    public ICollection<Attachment> Attachments { get; set; } = [];
}